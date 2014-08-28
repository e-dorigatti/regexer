using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {

    /** This exception is raised if an error occurs during the processing of the regular expression.
     */
    public class ParsingException : Exception {

        /** Creates a new instance of this exception.
         * 
         * \param message The message to show along with the exception.
         */
        public ParsingException( string message )
            : base( message ) { }


        /** Creates a new instance of this exception.
         * 
         * \param message The message to show along with the exception.
         * \param inner The exception which was thrown during the parsing stage.
         */
        public ParsingException( string message, Exception inner )
            : base( message, inner ) { }
    }
}
