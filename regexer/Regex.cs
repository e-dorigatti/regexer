using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    public class Regex {
        public string Pattern { get; private set; }         ///< The pattern matched by this regular expression.

        private Token _root;


        /** Compiles the pattern into a regular expression
         */
        public Regex( string pattern ) {
            this.Pattern = pattern;

            try {
                this._root = Token.Tokenize( pattern );
            }
            catch ( ParsingException ex ) {
                throw;
            }
            catch ( Exception ex ) {
                throw new ParsingException( "invalid pattern: " + pattern, ex );
            }
        }


        /** Returns the first match found in the input.
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


        /** Returns true if the input matches the pattern
         */
        public bool IsMatch( string input ) {
            return Match( input ).Success;
        }


        /** Finds all the matches in the input.
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


        public string ToLISPyString( ) {
            return _root.ToString( );
        }


        public override string ToString( ) {
            return Pattern;
        }
    }
}
