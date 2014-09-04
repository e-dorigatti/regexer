using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    class Program {
        static void Main( string[ ] args ) {
            string pattern = @"a{3}", input = "aaaaab";

            var rex = new Regex( pattern );
            Console.WriteLine( rex.ToLISPyString( ) );

            foreach ( string match in rex.Matches( input ) )
                Console.WriteLine( match );

            Console.ReadKey( true );
        }
    }
}
