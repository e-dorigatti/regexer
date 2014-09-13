using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    public class LookbehindToken : Token {
        public bool IsNegative { get; set; }
        public Token Target { get; set; }


        /** Creates a new LookbehindToken.
         * 
         *  The target token will be reversed; if you plan to edit the
         *  target token itself remember to manually reverse it at the end!
         */
        public LookbehindToken( string text, Token target )
            : base( TokenType.Lookbehind, text ) {

            this.IsNegative = text.EndsWith( "!" );
            this.Target = target;
            this.Target.Reverse( );
        }


        public override bool Matches( string input, ref int cursor ) {
            string reversed = new string( input.Substring( 0, cursor ).Reverse( ).ToArray( ) );
            int tempCursor = 0;

            bool matches = Target.Matches( reversed, ref tempCursor );

            if ( IsNegative )
                return !matches;
            else return matches;
        }

        
        public override bool CanBacktrack( string input, ref int cursor ) {
            return false;
        }


        protected override string printContent( ) {
            return Target.ToString( );
        }
    }
}
