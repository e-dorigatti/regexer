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
            try {
                IEnumerable<Token> tokens = findTokens( pattern );
                GroupToken root = regroupTokens( tokens );
                return convertOrs( root );
            }
            catch ( ParsingException ) {
                throw;
            }
            catch ( Exception ex ) {
                throw new ParsingException( "invalid pattern: " + pattern, ex );
            }
        }


        /** Extract a plain, unstructured sequence of tokens from the given pattern.
         * 
         *  When possible, yield tokens of the appropriate subclass; in any case
         *  Type is set to the appropriate TokenType value.
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
                        tokenStart = i;
                        if ( pattern[ i + 1 ] == '?' && pattern[ i + 2 ] == '<' ) {
                            while ( pattern[ i ] != '>' )
                                i += 1;
                        }

                        yield return new Token( TokenType.GroupStart, pattern.Substring( tokenStart, i - tokenStart + 1 ) );
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

                        // lazy?
                        if ( i < pattern.Length - 2 && pattern[ i + 1 ] == '?' )
                            i += 1;

                        yield return new Token( TokenType.Quantifier, pattern.Substring( tokenStart, i - tokenStart + 1 ) );
                        break;

                    case '|':
                        yield return new Token( TokenType.Or, "|" );
                        break;

                    case '\\':
                        yield return new LiteralToken( pattern.Substring( i, 2 ) );
                        i += 1;
                        break;

                    case '[':
                        tokenStart = i + 1;

                        // everything up to the first unescaped ']' is inside this token
                        while ( pattern[ i ] != ']' || pattern[ i - 1 ] == '\\' )
                            i += 1;

                        yield return new LiteralToken( pattern.Substring( tokenStart, i - tokenStart ) );
                        break;

                    case '^':
                        yield return new InputStartToken( );
                        break;

                    case '$':
                        yield return new InputEndToken( );
                        break;

                    default: // do not group adjacent characters
                        yield return new LiteralToken( pattern.Substring( i, 1 ) );
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
            int groupCount = 0;
            var groups = new Stack<GroupToken>( );
            var names = new HashSet<string>( );

            var current = new GroupToken( string.Empty, groupCount++ );
            groups.Push( current );

            foreach ( Token t in tokens ) {
                switch ( t.Type ) {
                    case TokenType.GroupStart:
                        var newGroup = new GroupToken( t.Text, groupCount++ );
                        if ( newGroup.Name != null ) {
                            if ( names.Contains( newGroup.Name ) )
                                throw new ParsingException( "multiple groups with the same name are not allowed" );
                            else names.Add( newGroup.Name );
                        }
                        
                        current.Content.Add( newGroup );
                        groups.Push( current );

                        current = newGroup;
                        break;

                    case TokenType.GroupEnd:
                        current = groups.Pop( );
                        break;

                    case TokenType.Quantifier:
                        Token target = current.Content.Last( );
                        current.Content.Remove( target );

                        var quantifier = new QuantifierToken( t.Text, target );
                        current.Content.Add( quantifier );
                        break;

                    default:
                        current.Content.Add( t );
                        break;
                }
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
         *  Or operators have the highest precedence, therefore we can safely replace
         *  any sequence of tokens which contain an or operator with an OrToken;
         *  splitting this sequence on or operators yields the list of alternatives
         *  for the OrToken itself.
         *  
         *  \param root The root of the syntax tree
         *  \return The tree with OrToken nodes inserted at appropriate locations.
         */
        private static Token convertOrs( Token root ) {
            if ( root is GroupToken ) {
                var group = root as GroupToken;
                if ( group.Content.Any( t => t.Type == TokenType.Or ) ) {
                    var or = new OrToken( );
                    or.Alternatives = split( group.Content, TokenType.Or )
                        .Select( l => l.Count > 1 ? new GroupToken( l ) : l.First( ) )
                        .Select( t => convertOrs( t ) )
                        .ToList( );

                    group.Content.Clear( );
                    group.Content.Add( or );
                }
                else {
                    group.Content = group.Content
                        .Select( t => convertOrs( t ) )
                        .ToList( );
                }
            }
            else if ( root is QuantifierToken ) {
                var quantifier = root as QuantifierToken;
                quantifier.Target = convertOrs( quantifier.Target );
            }

            return root;
        }


        /** Splits a list of tokens; the delimiter is any token with the specified type.
         * 
         *  Just like string.Split; the delimiter is not included and empty sub lists
         *  can be retured.
         *  
         *  \param tokens The list of tokens to split
         *  \param type The type of the delimiter tokens
         *  \returns A sequence of sub lists delimited by tokens of the specified type.
         */
        private static IEnumerable<List<Token>> split( List<Token> tokens, TokenType type ) {
            var accumulator = new List<Token>( );

            foreach ( Token t in tokens ) {
                if ( t.Type == type ) {
                    yield return accumulator;
                    accumulator = new List<Token>( );
                }
                else accumulator.Add( t );
            }

            yield return accumulator;
        }
    }
}
