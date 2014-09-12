using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer {
    /** The LiteralToken matches all the character contained or not contained
     *  in a specified set.
     *  
     *  The pattern of accepted characters can be specified in different forms:
     *  
     *   - As a character range: "d-g" matches "defg"
     *  
     *   - Specific characters: "aeiou" matches the set of vowels
     *  
     *   - A combination of the previous forms: "a-zA-Z02468" matches
     *     lowercase and uppercase letters and all the even numbers
     *  
     *   - A caret before the pattern specifies a negative matching:
     *     "^aeiou" matches everything but vowels (i.e. consonants :) )
     *   
     *   - A dot matches every character.
     *  
     *   - As a character class; list of available character classes and
     *     their meaning:
     *     
     *     - <b>\\w</b>: a-zA-Z0-9_
     *     - <b>\\W</b>: ^a-zA-Z0-9_
     *     - <b>\\d</b>: 0-9
     *     - <b>\\D</b>: ^0-9
     *     - <b>\\s</b>: -space- -tab-
     *     - <b>\\S</b>: ^ -space- -tab-
     */
    public class LiteralToken : Token {

        //! Possible types of matching.
        public enum eMatchType {
            Positive,   ///< Positive matching: the input must be contained in the set of characters.
            Negative    ///< Negative matching: the input must not be contained in the set of characters.
        }

        private static readonly Dictionary<string, string> _charClasses =
            new Dictionary<string, string>( ) { 
                { "\\w", buildFromRanges( "a-zA-Z0-9_" ) },
                { "\\W", buildFromRanges("^a-zA-Z0-9_") },
                { "\\d", buildFromRanges( "0-9" ) },
                { "\\D", buildFromRanges("^0-9") },
                { "\\s", " \t" },
                { "\\S", "^ \t" },
            };

        private const string
            _lowercase = "abcdefghijklmnopqrstuvwxyz",
            _uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            _digits = "0123456789";

        public string Pattern { get; private set; }         ///< The set of allowed / disallowed characters 
        public eMatchType MatchType { get; private set; }   ///< Type of match: positive or negative. 

        private bool _matchesEverything = false;

        /** Create a new literal token.
         * 
         *  \param pattern Pattern specifying the set of allowed/disallowed characters.
         */
        public LiteralToken( string pattern )
            : base( TokenType.Literal, pattern ) {

            /// \bug this fails when there is an escaped dot before an unescaped one
            int dotIndex = pattern.IndexOf( '.' );
            if ( dotIndex == 0 || ( dotIndex > 0 && pattern[ dotIndex - 1 ] != '\\' ) ) {
                _matchesEverything = true;
                return;
            }

            // positive or negative matching?
            if ( pattern[ 0 ] == '^' ) {
                MatchType = eMatchType.Negative;
                pattern = pattern.Substring( 1 );
            }
            else MatchType = eMatchType.Positive;

            // replace character classes with their pattern
            foreach ( var kvp in _charClasses )
                pattern = pattern.Replace( kvp.Key, kvp.Value );

            try {
                this.Pattern = buildFromRanges( pattern ).Replace( "\\]", "]" );
            }
            catch ( ParsingException ) {
                throw;
            }
            catch ( Exception ex ) {
                throw new ParsingException( "invalid pattern: " + pattern, ex );
            }
        }


        public override bool Matches( string input, ref int cursor ) {
            if ( cursor < input.Length && Matches( input[ cursor ] ) ) {
                cursor += 1;
                return true;
            }
            else return false;
        }


        public override bool CanBacktrack( string input, ref int cursor ) {
            return false;
        }


        /** Explicitly express character ranges.
         * 
         *  For example, ab-ef12-4 becomes abcdef1234
         * 
         *  \param range  One or more character ranges. 
         *  \returns The range in expanded form.
         */
        private static string buildFromRanges( string range ) {
            string pattern = string.Empty;

            List<string> parts = splitRanges( range );
            for ( int i = 0; i < parts.Count - 1; i++ ) {
                pattern += parts[ i ].Substring( 0, parts[ i ].Length - 1 );

                char first = parts[ i ].Last( ),
                    last = parts[ i + 1 ].First( );

                string source;
                if ( _lowercase.Contains( first ) && _lowercase.Contains( last ) )
                    source = _lowercase;
                else if ( _uppercase.Contains( first ) && _uppercase.Contains( last ) )
                    source = _uppercase;
                else if ( _digits.Contains( first ) && _digits.Contains( last ) )
                    source = _digits;
                else throw new ParsingException( "invalid range: " + first + " - " + last );

                if ( source.IndexOf( first ) > source.IndexOf( last ) )
                    throw new ParsingException( "invalid range: " + first + " - " + last );

                // characters in source between first and last (included)
                int indexFirst = source.IndexOf( first ),
                    indexLast = source.IndexOf( last );
                pattern += source.Substring( indexFirst, indexLast - indexFirst + 1 );

                parts[ i + 1 ] = parts[ i + 1 ].Substring( 1 );
            }

            return pattern + parts.Last( );
        }


        /** Split a string around unescaped dashes.
         * 
         *  \param pattern The pattern.
         *  \returns The list of parts.
         */
        private static List<string> splitRanges( string pattern ) {
            var parts = new List<string>( );
            var sb = new StringBuilder( );

            for ( int i = 0; i < pattern.Length; i++ ) {

                // if there's a dash in position 0 then add it, otherwise check the previous character as well
                if ( pattern[ i ] == '-' && ( i == 0 || pattern[ i - 1 ] != '\\' ) ) {
                    parts.Add( sb.ToString( ) );
                    sb.Clear( );
                }
                else sb.Append( pattern[ i ] );
            }

            parts.Add( sb.ToString( ) );
            return parts;
        }


        /** Checks if the pattern matches the given character.
         * 
         *  \param c  The character. 
         *  \returns True if the character is/is not in the set of accepted characters.
         */
        public bool Matches( char c ) {
            if ( _matchesEverything )
                return true;
            else if ( MatchType == eMatchType.Positive )
                return Pattern.Contains( c );
            else return !Pattern.Contains( c );
        }
    }
}
