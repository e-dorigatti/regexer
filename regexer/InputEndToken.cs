using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {

    /** The InputEndToken makes sure that the match ends and the end of the
     *  input string.
     *  
     *  For example, the input "aaaa" when matched by the pattern "a$" produces
     *  only one match, the last "a", whereas the pattern "a" matches all the "a"s;
     *  the pattern "a$a" does not match at all.
     *  
     *  \todo add support for single line/multiline regexes
     */
    class InputEndToken : Token {

        /* Creates a new InputStartToken
         */
        public InputEndToken( )
            : base( TokenType.InputEnd, "$" ) { }


        public override bool Matches( string input, ref int cursor ) {
            return cursor == input.Length;
        }


        public override bool CanBacktrack( string input, ref int cursor ) {
            return false;
        }


        protected override string printContent( ) {
            return string.Empty;
        }
    }
}
