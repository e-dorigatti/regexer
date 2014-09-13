using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    public class LookaheadToken : Token {

        public bool IsNegative { get; set; }

        public Token Target { get; set; }

        public LookaheadToken( string text, Token target )
            : base( TokenType.Lookahead, text ) {

            this.IsNegative = text.EndsWith( "!" );
            this.Target = target;
        }

        public override bool Matches( string input, ref int cursor ) {
            int start = cursor;

            bool matches = Target.Matches( input, ref cursor );
            cursor = start;

            // return IsNegative ^ matches;
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

