using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    public partial class Token {


        /** Build an abstract syntax tree representing the given regex.
         * 
         *  \param pattern Regex pattern.
         *  \return The root of the tree.
         */
        public static Token Tokenize( string pattern ) {
            IEnumerable<Token> tokens = findTokens( pattern );
            GroupToken root = regroupTokens( tokens );
            return convertToPrefixOrs( root );
        }


        /** Extract a plain, unstructured sequence of tokens from the given pattern.
         * 
         *  \param pattern The regex pattern.
         *  \return A sequence of tokens.
         */
        private static IEnumerable<Token> findTokens( string pattern ) {
            int tokenStart;

            for ( int i = 0; i < pattern.Length; i++ ) {
                char c = pattern[ i ];

                switch ( c ) {
                    case '(':
                        yield return new Token( TokenType.GroupStart, "(" );
                        break;

                    case ')':
                        yield return new Token( TokenType.GroupEnd, ")" );
                        break;

                    case '*':
                    case '+':
                    case '?':
                        string text;
                        if ( c != '?' && i < pattern.Length - 1 && pattern[ i + 1 ] == '?' ) {
                            text = c + "?";
                            i += 1;
                        }
                        else text = "" + c;

                        yield return new Token( TokenType.Quantifier, text );
                        break;

                    case '{':
                        tokenStart = i;
                        while ( pattern[ i ] != '}' )
                            i += 1;

                        yield return new Token( TokenType.Quantifier, pattern.Substring( tokenStart, i - tokenStart + 1 ) );
                        break;

                    case '|':
                        yield return new Token( TokenType.Or, "|" );
                        break;

                    case '\\':
                        yield return new Token( TokenType.Literal, pattern.Substring( i, 2 ) );
                        i += 1;
                        break;

                    case '[':
                        tokenStart = i + 1;

                        // everything up to the first unescaped ']' is inside this token
                        while ( pattern[ i ] != ']' || pattern[ i - 1 ] == '\\' )
                            i += 1;

                        yield return new Token( TokenType.Literal, pattern.Substring( tokenStart, i - tokenStart ) );
                        break;

                    default: // do not group adjacent characters
                        yield return new Token( TokenType.Literal, pattern.Substring( i, 1 ) );
                        break;
                }
            }
        }


        /** Recursively transform a plain sequence of Token into a tree-like structure,
         *  transforming them into the appropriate subclass.
         *  
         *  Token themselves as returned by findTokens() are not of much use because
         *  they often refer to other tokens located near them, either before or after.
         *  Also, regexes do have a structure, given by round brackets; in order to
         *  recognise and preserve this structure some further processing is needed
         *  to the stream of tokens.
         *  
         *  A tree is the perfect data type to represent the grammar and the structure
         *  of a regex, where node types specify the meaning and node children represent
         *  the "arguments" of each component.
         *  
         *  \param tokens A sequence of tokens.
         *  \return An organised tree.
         */
        private static GroupToken regroupTokens( IEnumerable<Token> tokens ) {
            var groups = new Stack<GroupToken>( );

            var current = new GroupToken( );
            groups.Push( current );

            foreach ( Token t in tokens ) {
                if ( t.Type == TokenType.GroupStart ) {
                    var newGroup = new GroupToken( );
                    current.Content.Add( newGroup );
                    groups.Push( current );

                    current = newGroup;
                }
                else if ( t.Type == TokenType.GroupEnd ) {
                    current = groups.Pop( );
                }
                else if ( t.Type == TokenType.Quantifier ) {
                    Token target = current.Content.Last( );
                    current.Content.Remove( target );

                    var quantifier = new QuantifierToken( t.Text, target );
                    current.Content.Add( quantifier );
                }
                else if ( t.Type == TokenType.Literal ) {
                    var literal = new LiteralToken( t.Text );
                    current.Content.Add( literal );
                }
                else current.Content.Add( t );
            }

            if ( groups.Count > 1 )
                throw new ParsingException( "unbalanced parenthesis" );

            return groups.Pop( );
        }


        /** Recursively convert or operators from infix to prefix notation.
         * 
         *  Or operators are specified using an infix notation which is inconvenient
         *  in the tree-like structure we are building; we have to convert this
         *  notation to a prefix style, creating an OrToken node in the tree.
         *  
         *  Since or operators are among the highest precedence operators in regexes
         *  we can safely replace all the GroupToken nodes which contain at least
         *  an or operation with an OrToken.
         *  
         *  \param root The root of the syntax tree
         *  \return The tree with OrToken nodes inserted at appropriate locations.
         *  Note that the root of the tree changes type in the process; this happens
         *  because a GroupToken might be replaced by an OrToken.
         */
        private static Token convertToPrefixOrs( GroupToken root ) {
            var orToken = new OrToken( );
            var current = new GroupToken( );

            foreach ( Token t in root.Content ) {
                if ( t.Type == TokenType.Or ) {
                    if ( !current.Content.Any( ) )
                        throw new ParsingException( "invalid or sequence" );

                    orToken.Alternatives.Add( current );
                    current = new GroupToken( );
                }
                else if ( t.Type == TokenType.Group ) {
                    Token converted = convertToPrefixOrs( t as GroupToken );
                    current.Content.Add( converted );
                }
                else if ( t.Type == TokenType.Quantifier ) {
                    var quantifier = t as QuantifierToken;
                    if ( quantifier.Target is GroupToken )
                        quantifier.Target = convertToPrefixOrs( quantifier.Target as GroupToken );

                    current.Content.Add( quantifier );
                }
                else current.Content.Add( t );
            }

            if ( !current.Content.Any( ) )
                throw new ParsingException( "invalid or sequence" );

            orToken.Alternatives.Add( current );
            return compressToken( orToken );
        }


        /** Recursively replace all OrToken / GroupToken which have a single-token content
         *  with that content.
         *  
         *  To keep the code simple, some functions might insert OrToken / GroupToken
         *  whose content consists in only one token; while this is technically
         *  correct it is also quite unpleasant and might add unnecessary complexity
         *  to the next stages of the processing. In this case, the token will simply
         *  be replaced with its content.
         *  
         *  \param root The token to compress.
         *  \return The same token compressed.
         */
        private static Token compressToken( Token root ) {
            List<Token> tokens;

            if ( root is GroupToken )
                tokens = ( root as GroupToken ).Content;
            else if ( root is OrToken )
                tokens = ( root as OrToken ).Alternatives;
            else return root;

            Token result;
            if ( tokens.Count == 1 )
                result = compressToken( tokens.First( ) );
            else {
                for ( int i = 0; i < tokens.Count; i++ )
                    tokens[ i ] = compressToken( tokens[ i ] );

                if ( root is GroupToken )
                    ( root as GroupToken ).Content = tokens;
                else ( root as OrToken ).Alternatives = tokens;
                result = root;
            }

            return result;
        }
    }
}
