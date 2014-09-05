using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    public static class Extension {
        public static RegexMatch Match( this string input, string pattern ) {
            return Regex.Match( pattern, input );
        }

        public static bool IsMatch( this string input, string pattern ) {
            return Regex.IsMatch( pattern, input );
        }

        public static IEnumerable<RegexMatch> Matches( this string input, string pattern ) {
            return Regex.Matches( pattern, input );
        }

        public static string Replace( this string input, string pattern, Func<RegexMatch, string> evaluator ) {
            return Regex.Replace( pattern, input, evaluator );
        }
    }
}
