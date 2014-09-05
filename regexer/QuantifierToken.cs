using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {


    /** The QuantifierToken is used when a pattern needs to appear several times
     *  in the input, ranging from a minimum to a maximum number of occurrences
     *  (this number can be infinite).
     *  
     *  The syntax is as follows (where a is the target pattern):
     *  
     *    - a*       <b>min:</b> 0    <b>max:</b> inf
     *    - a+       <b>min:</b> 1    <b>max:</b> inf
     *    - a{,n}    <b>min:</b> 0    <b>max:</b> n
     *    - a{n,}    <b>min:</b> n    <b>max:</b> inf
     *    - a{m, n}  <b>min:</b> m    <b>max:</b> n
     *  
     *  Quantifiers can either be greedy or lazy. A greedy quantifier (the default
     *  behavior) matches the target as many times as possible whereas a lazy
     *  quantifier matches the target as few times as possible. The lazy behavior
     *  can be specified by appending a question mark "?" after the quantifier
     *  itself. For example, here's the difference when the input is "aabaabaab":
     *  
     *   - Consider the pattern "(aab)+" (greedy quantifier): match is "aabaabaab"
     *     
     *   - Consider the pattern "(aab)+?" (lazy quantifier): match is "aab"
     */
    public class QuantifierToken : Token {
        public int MinOccurrences { get; set; }     ///< Minimum number of occurrences of the target pattern.
        public int MaxOccurrences { get; set; }     ///< Maximum number of occurrences of the target pattern.
        public bool IsLazy { get; set; }            ///< Lazy/Greedy behavior (defaults to greedy) 
        public Token Target { get; set; }           ///< Target of the quantifier. 


        /** Cursor locations which allow the quantifier to backtrack.
         * 
         *  Greedy quantifiers will push here all of the available locations found while matching,
         *  and pop them one by one when backtracking.
         *  
         *  Lazy quantifiers will only push the last location because they backtrack by trying to
         *  match the input one more time.
         */
        private Stack<int> _backtrackingPoints;
        private int _backtrackingMatches;       ///< Number of matches when backtracking lazy quantifiers


        /** Creates a new quantifier token.
         * 
         * \param content  String specifying the minimum and maximum occurences of the pattern. 
         * \param target  Target of the quantifier. 
         */
        public QuantifierToken( string content, Token target )
            : base( TokenType.Quantifier, content ) {

            this.Target = target;
            this._backtrackingPoints = new Stack<int>( );

            try {
                buildQuantifier( content );
            }
            catch ( ParsingException ex ) {
                throw;
            }
            catch ( Exception ex ) {
                throw new ParsingException( "invalid quantifier " + content, ex );
            }
        }


        public override bool Matches( string input, ref int cursor ) {
            _backtrackingPoints.Clear( );
            _backtrackingMatches = 0;

            int matches = 0, start = cursor;
            while ( matches <= MaxOccurrences ) {

                if ( matches >= MinOccurrences )
                    _backtrackingPoints.Push( cursor );

                if ( Target.Matches( input, ref cursor ) ) {
                    matches += 1;

                    if ( IsLazy )
                        return true;
                }
                // Since we do not match anymore go back just after the last match (if possible)
                else return CanBacktrack( input, ref cursor );
            }

            cursor = start;
            return false;
        }


        public override bool CanBacktrack( string input, ref int cursor ) {
            if ( !_backtrackingPoints.Any( ) )
                return false;

            int start = cursor;
            cursor = _backtrackingPoints.Pop( );

            // greedy quantifiers have already matched the input, they just need to go back
            if ( !IsLazy ) return true;

            // lazy quantifiers need to match one more time (if possible)
            if ( Target.Matches( input, ref cursor ) ) {
                _backtrackingMatches += 1;
                if ( _backtrackingMatches <= MaxOccurrences ) {
                    _backtrackingPoints.Push( cursor );
                    return true;
                }
            }

            // no match or maximum number of matches reached
            cursor = start;
            return false;
        }


        /** Parse the quantifier and set the fields to the appropriate value.
         * 
         * \param content The quantifier
         */
        private void buildQuantifier( string content ) {
            if ( content.Length > 1 && content.EndsWith( "?" ) ) {
                this.IsLazy = true;
                content = content.Substring( 0, content.Length - 1 );
            }

            if ( content.StartsWith( "?" ) ) {
                MinOccurrences = 0;
                MaxOccurrences = 1;
            }
            else if ( content.StartsWith( "+" ) ) {
                MinOccurrences = 1;
                MaxOccurrences = int.MaxValue;
            }
            else if ( content.StartsWith( "*" ) ) {
                MinOccurrences = 0;
                MaxOccurrences = int.MaxValue;
            }
            else if ( content.StartsWith( "{" ) && content.EndsWith( "}" ) ) {
                content = content.Substring( 1, content.Length - 2 );
                string[ ] parts = content.Split( ',' );

                if ( parts.Length == 1 ) {
                    MinOccurrences = MaxOccurrences = int.Parse( parts[ 0 ] );
                    return;
                }

                if ( parts[ 0 ].Length == 0 )
                    MinOccurrences = 0;
                else MinOccurrences = int.Parse( parts[ 0 ] );

                if ( parts[ 1 ].Length == 0 )
                    MaxOccurrences = int.MaxValue;
                else MaxOccurrences = int.Parse( parts[ 1 ] );
            }
            else throw new ParsingException( "unrecognized quantifier: " + content );
        }


        /** String representation of this Token, including minumum and maximum occurrences
         *  and the target of the quantifier.
         *  
         *  \returns The string representation of this token.
         */
        protected override string printContent( ) {
            return string.Format( "min: {0}\nmax: {1}\ntarget: {2}", MinOccurrences,
                MaxOccurrences == int.MaxValue ? "-" : MaxOccurrences.ToString( ), Target );
        }
    }
}
