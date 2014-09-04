using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {

    /** The InputStartToken makes sure that the match starts at the beginning
     *  of the input string.
     *  
     *  For example, the input "aaaa" when matched by the pattern "^a" produces
     *  only one match, i.e. "a", whereas the pattern "a" matches all four as;
     *  the pattern "a^a" does not match at all.
     *  
     *  \todo add single line/multiline regexes support
     */
    class InputStartToken : Token {

        /** Creates a new InputStartToken
         */
        public InputStartToken( ) 
            : base( TokenType.InputStart, "^" ) { }

        
        public override bool Matches( string input, ref int cursor ) {
            return cursor == 0;
        }


        public override bool CanBacktrack( string input, ref int cursor ) {
            return false;
        }


        protected override string printContent( ) {
            return string.Empty;
        }
    }
}
