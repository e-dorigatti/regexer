using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using regexer;
using System.Linq;

namespace regexerTest {

    [TestClass( )]
    public class LookaroundTest {

        [TestMethod( )]
        public void PositiveLookaheadTest( ) {
            string pattern = "a(?=b{2,})";  // an 'a' followed by at least 2 'b's

            Assert.IsFalse( Regex.IsMatch( pattern, "ac" ) );
            Assert.IsFalse( Regex.IsMatch( pattern, "acc" ) );
            Assert.IsFalse( Regex.IsMatch( pattern, "ab" ) );
            Assert.AreEqual( "a", Regex.Match( pattern, "abb" ).Value );

            Assert.IsFalse(Regex.IsMatch("a(?=b+)a", "abbba"));
        }

        [TestMethod( )]
        public void NegativeLookaheadTest( ) {
            string pattern = "a(?!b{2,})";  // any 'a' not followed by more than 2 'b's (or anything else)

            Assert.AreEqual( "a", Regex.Match( pattern, "ac" ).Value );
            Assert.AreEqual( "a", Regex.Match( pattern, "acc" ).Value );
            Assert.AreEqual( "a", Regex.Match( pattern, "ab" ).Value );
            Assert.IsFalse( Regex.IsMatch( pattern, "abb" ) );
        }

        [TestMethod( )]
        public void NestedLookaroundsTest( ) {
            var good = Token.Tokenize( "(?= a ( b ( c ) ) )" );
            try {
                var bad = Token.Tokenize( "(?= ddd ( cc ( dd (?! ff ) d  ) c ) a ) b" );
                Assert.Fail( "no exception thrown for nested lookarounds" );
            }
            catch ( ParsingException ) { }
        }

        [TestMethod( )]
        public void PositiveLookbehindTest( ) {
            string pattern = @"(?<=ab+c)d+";

            Assert.AreEqual( "d", Regex.Match( pattern, "abcd" ).Value );
            Assert.AreEqual( "dd", Regex.Match( pattern, "abbbbbcdd" ).Value );
            Assert.IsFalse( Regex.IsMatch( pattern, "acd" ) );
            Assert.IsFalse( Regex.IsMatch( pattern, "abc" ) );
        }

        [TestMethod( )]
        public void NegativeLookbehindTest( ) {
            string pattern = "(?<!a)b";
            var matches = Regex.Matches( pattern, "abbb" ).ToList( );

            Assert.AreEqual( 2, matches.Count );
            Assert.IsTrue( matches.All( m => m.Value == "b" ) );
            Assert.AreEqual( 2, matches[ 0 ].Start );
            Assert.AreEqual( 3, matches[ 1 ].Start );
        }
    }
}
