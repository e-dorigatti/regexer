using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {

    /** Contains the information about a matched group in the input.
     * 
     *  Groups are portions of the pattern enclosed in round brackets;
     *  information regarding the portion of the input matched by every
     *  group is collected and stored in a RegexGroup instance.
     */
    public class RegexGroup {
        public int Start { get; private set; }      ///< Starting position in the input.
        public int End { get; private set; }        ///< Ending position in the input.
        public string Value { get; private set; }   ///< Portion of the input matched by this group.
        public int Index { get; private set; }      ///< Index of the group (in order or appearance in the pattern).
        public string Name { get; private set; }    ///< Name of the group or null if not specified


        public RegexGroup( int start, int end, string input, int index = -1, string name = null ) {
            this.Start = start;
            this.End = end;
            this.Value = input.Substring( start, end - start );
            this.Index = index;
            this.Name = name;
        }


        public override string ToString( ) {
            return Value;
        }
    }
}
