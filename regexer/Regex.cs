using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    public class Regex {
        public string Pattern { get; private set; }

        private Token _root;

        public Regex( string pattern ) {
            this.Pattern = pattern;
            this._root = Token.Tokenize( pattern );
        }

        public IEnumerable<string> Matches( string input ) {
            for ( int i = 0; i < input.Length; i++ ) {
                int cursor = i;  // we do not want our counter to be modified, unless there is a match...

                if ( _root.Matches( input, ref cursor ) ) {
                    yield return input.Substring( i, cursor - i );
                    i = cursor - 1; // in which case, jump over it
                    
                    // (note that i will be incremented before the next iteration starts, thus we
                    // have to subract one or we will skip a character)
                }
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
