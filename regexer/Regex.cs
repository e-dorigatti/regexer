using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    public class Regex {
        public string Pattern { get; private set; }         ///< The pattern matched by this regular expression.

        private Token _root;


        /** Compiles the pattern into a regular expression.
         * 
         *  \param pattern The pattern to compile.
         */
        public Regex( string pattern ) {
            this.Pattern = pattern;

            try {
                this._root = Token.Tokenize( pattern );
            }
            catch ( ParsingException ) {
                throw;
            }
            catch ( Exception ex ) {
                throw new ParsingException( "invalid pattern: " + pattern, ex );
            }
        }


        /** Returns the first match found in the input.
         * 
         *  If there is no match, a RegexMatch will be returned whose
         *  Success is set to false.
         * 
         *  \param input The input string.
         *  \return The first match. If the match did not succeed,
         *  RegexMatch.Success will be set to false.
         */
        public RegexMatch Match( string input ) {
            for ( int i = 0; i < input.Length; i++ ) {
                int cursor = i;

                if ( _root.Matches( input, ref cursor ) ) {
                    var match = new RegexMatch( i, cursor, input );
                    buildMatch( _root, match );
                    return match;
                }
            }

            return new RegexMatch( 0, 0, string.Empty, false );
        }


        /** Returns true if the input matches the pattern.
         * 
         *  The match is not constrained to appear at the beginning of the
         *  input and can appear in the middle of it.
         *  
         *  \param input The input string.
         *  \return True if the pattern was matched at least once.
         */
        public bool IsMatch( string input ) {
            return Match( input ).Success;
        }


        /** Finds all the matches in the input.
         * 
         *  Overlapping matches will not be reported, so that "aaa", when
         *  matched with "a+" will only produce one match (i.e. "aaa") and
         *  not three (i.e. "aaa", "aa" and "a").
         * 
         *  \param input The input string.
         *  \return A sequence of RegexMatch.
         */
        public IEnumerable<RegexMatch> Matches( string input ) {
            for ( int i = 0; i < input.Length; i++ ) {
                int cursor = i;

                if ( _root.Matches( input, ref cursor ) ) {
                    var match = new RegexMatch( i, cursor, input );
                    buildMatch( _root, match );

                    yield return match;

                    i = cursor - 1; // jump over the whole match
                    // (note that i will be incremented before the next iteration starts, thus we
                    // have to subract one or we will skip a character)
                }
            }
        }


        /** Recursively traverse the tree collecting all matches from GroupTokens.
         */
        private void buildMatch( Token root, RegexMatch accumulator ) {
            if ( root is GroupToken ) {
                var group = root as GroupToken;

                if ( group.Index >= 0 )
                    accumulator.Groups.Add( group.Match );

                foreach ( Token t in group.Content )
                    buildMatch( t, accumulator );
            }
            else if ( root is QuantifierToken ) {
                var quantifier = root as QuantifierToken;
                buildMatch( quantifier.Target, accumulator );
            }
            else if ( root is OrToken ) {
                var or = root as OrToken;
                foreach ( Token t in or.Alternatives )
                    buildMatch( t, accumulator );
            }
        }


        /** Replaces all the matches with the value returned by a function.
         * 
         *  \param input The input string.
         *  \param evaluator A function taking a RegexMatch as argument and returning a string.
         *  \return The input string where all the matches have been replaced.
         */
        public string Replace( string input, Func<RegexMatch, string> evaluator ) {
            var sb = new StringBuilder( );

            int previous_end = 0;
            foreach ( var match in Matches( input ) ) {
                string replacement = evaluator( match );

                sb.Append( input.Substring( previous_end, match.Start - previous_end ) );
                sb.Append( replacement );
                previous_end = match.End;
            }
            sb.Append( input.Substring( previous_end ) );

            return sb.ToString( );
        }


        /** Replaces all the matches with the specified string.
         * 
         *  You can reference captured groups by index in the replacement string
         *  enclosing them in curly brackets, so that {0} will be replaced with
         *  the whole match, {1} with the first captured group, etc.
         *  
         *  \param input The input string.
         *  \param replacement The replacement string.
         *  \return The input string where all the matches have been replaced.
         */
        public string Replace( string input, string replacement ) {
            return Replace( input, m => string.Format( replacement, m.Groups.ToArray( ) ) );
        }


        public static RegexMatch Match( string pattern, string input ) {
            return new Regex( pattern ).Match( input );
        }

        public static bool IsMatch( string pattern, string input ) {
            return new Regex( pattern ).IsMatch( input );
        }

        public static IEnumerable<RegexMatch> Matches( string pattern, string input ) {
            return new Regex( pattern ).Matches( input );
        }

        public static string Replace( string pattern, string input, Func<RegexMatch, string> evaluator ) {
            return new Regex( pattern ).Replace( input, evaluator );
        }

        public string ToLISPyString( ) {
            return _root.ToString( );
        }

        public override string ToString( ) {
            return Pattern;
        }
    }
}
