using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {

    /** Contains the information about a match, including the matched groups
     */
    public class RegexMatch : RegexGroup {

        public List<RegexGroup> Groups { get; private set; }    ///< Collection of matched groups as specified in the pattern
        public bool Success { get; private set; }               ///< Whether the match has been successful or not.


        public RegexMatch( int start, int end, string input, bool success = true )
            : base( start, end, input ) {

            this.Groups = new List<RegexGroup>( );
            this.Success = success;
        }
    }
}
