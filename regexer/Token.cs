using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {

    /** Base class for the tokens, representing a generic piece of pattern.
     */
    public partial class Token {

        /** Recognized types of token.
         * 
         *  Most of these types have their own class, inheriting from this, which
         *  will be used to build a syntax tree and, eventually, to perform the
         *  actual matching. At first plain tokens are extracted from the pattern,
         *  which will then be converted to the appropriate subclass.
         */
        public enum TokenType {
            Unknown,        ///< Unknown token. Default value, should never be used.
            Literal,        ///< This token matches one of a set of characters.
            Quantifier,     ///< This token specifies how many times a pattern must occur.
            GroupStart,     ///< A new group starts with this token.
            GroupEnd,       ///< A group ends with this token.
            Group,          ///< This token is a sequence of patterns matched sequentially.
            Or,             ///< This token contains a list of alternative choices.
            InputStart,     ///< This token matches only if the cursor is at the start of the string.
            InputEnd,       ///< This token matches only if the cursor is at the end of the string.
            Lookahead,      ///<
            Lookbehind,     ///<
        }


        public TokenType Type { get; set; }     ///< Specifies the type of information represented by this token.
        public string Text { get; set; }        ///< Part of the pattern which this token refers to.


        /** Create a new Token.
         * 
         *  \param type  Type of this token. 
         *  \param text  Text of this token. 
         */
        public Token( TokenType type, string text ) {
            this.Type = type;
            this.Text = text;
        }


        /** Determines if the given input, starting at the cursor, matches the pattern of
         *  this token.
         *  
         *  If there is a match, the cursor should be positioned at the next character
         *  after the end of the match; otherwise, the cursor's position should not
         *  be changed.
         * 
         *  \param input The input to test against this token
         *  \param cursor The position from where to start. If there is a match, the
         *  cursor will be moved after the match itself
         *  \return True if a match is found, false otherwise
         */
        public virtual bool Matches( string input, ref int cursor ) {
            throw new NotImplementedException( );
        }


        /** If possible, perform backtracking.
         *  
         *  Even though the input successfully matches this token, it might be possible that it
         *  does not match one of the next tokens, making the whole match fail. This can happen
         *  even if the input matches the regular expression! When matching an input, some choices
         *  have to be taken and some of them might well be wrong; this method allows tokens to
         *  take another path in case the matching fails.
         *  
         *  The cursor should be moved to the next available backtracking position if it is possible
         *  to do so, otherwise its value should not be changed. Note that this method should be 
         *  stateful as it might be called multiple times in case multiple choices were made.
         * 
         *  \param input The input that we are trying to match
         *  \param cursor The current position in the input. Should be modified to the next available
         *  backtracking position
         *  \return True if it was possible to backtrack, false otherwise.
         */
        public virtual bool CanBacktrack( string input, ref int cursor ) {
            throw new NotImplementedException( );
        }


        /** Reverse the pattern matched by this token to allow backwards matching.
         * 
         *  For example "ab+c" reversed will become "cb+a"; this is useful when implementing
         *  lookbehinds.
         */
        public virtual void Reverse( ) {
            return;
        }


        /** Helper method used to create the string representation of the content of this token.
         * 
         *  It defaults to the text but can be overridden and cusotmized for more complex tokens.
         *  There is no need to indent the string, ToString will do that for you.
         *  
         *  \return This should be overridden to return the relevant information for the current
         *  token.
         */
        protected virtual string printContent( ) {
            return Text;
        }


        /** String representation of this Token in LISP-inspired and indented form, including
         *  the type of the token and its content.
         *
         *  \return A string representing this token.
         *  \see printContent
         */
        public override string ToString( ) {
            string text = this.printContent( )
                .Split( '\n' )
                .Select( s => "\t" + s )
                .Aggregate( string.Empty, ( a, s ) => a + s + "\n" )
                .Trim( );

            return string.Format( "({0}\n\t{1})\n", Type, text );
        }
    }
}
