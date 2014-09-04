using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {

    /** The OrToken represents a choice between several patterns.
     * 
     *  The input shall contain only one of these patterns to be accepted by this token.
     *  The pattern 'a|b+|c' is compiled to an OrToken with three alternatives:
     *  a, b+ or c. Only one of these patterns need to appear in the input string.
     */
    public class OrToken : Token {

        public List<Token> Alternatives { get; set; }   ///< List of alternatives; only one needs to appear in the input.

        /** Creates a new OrToken.
         */
        public OrToken( )
            : base( TokenType.Or, "|" ) {

            Alternatives = new List<Token>( );
        }


        public override bool Matches( string input, ref int cursor ) {
            int start = cursor;

            for ( int i = 0 ; i < Alternatives.Count; i++ ) {
                Token t = Alternatives[ i ];

                if ( t.Matches( input, ref cursor ) )
                    return true;
                else cursor = start;
            }

            return false;
        }


        public override bool CanBacktrack( string input, ref int cursor ) {
            return false;
        }


        /** String representation of this token; all the alternatives are included.
         * 
         * \returns The string representation of this token.
         */
        protected override string printContent( ) {
            var sb = new StringBuilder( );
            foreach ( Token t in Alternatives )
                sb.AppendLine( t.ToString( ) );
            return sb.ToString( );
        }
    }
}
