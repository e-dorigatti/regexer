using regexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace regexerTest {

    [TestClass( )]
    public class RegexTest {

        [TestMethod( )]
        public void RegexIsMatchTest( ) {
            Assert.AreEqual( true, new Regex( "a+" ).IsMatch( "bbaa" ) );
            Assert.AreEqual( false, new Regex( "a+" ).IsMatch( "bb" ) );
        }

        [TestMethod( )]
        public void RegexMatchTest( ) {
            var match = new Regex( "a+" ).Match( "bbaa" );
            Assert.AreEqual( true, match.Success );
            Assert.AreEqual( 1, match.Groups.Count );
            Assert.AreEqual( "aa", match.Groups.First( ).Value );

            Assert.AreEqual( false, new Regex( "a+" ).Match( "bb" ).Success );
        }

        [TestMethod( )]
        public void RegexGroupingTest( ) {
            var match = new Regex( "((a+)b|(b+)a)+" ).Match( "aabbbaaaab" );
            Assert.AreEqual( true, match.Success );
            Assert.AreEqual( 4, match.Groups.Count );

            Assert.AreEqual( "aabbbaaaab", match.Groups[ 0 ].Value );
            Assert.AreEqual( 0, match.Groups[ 0 ].Index );

            Assert.AreEqual( "aaab", match.Groups[ 1 ].Value );
            Assert.AreEqual( 1, match.Groups[ 1 ].Index );

            Assert.AreEqual( "aaa", match.Groups[ 2 ].Value );
            Assert.AreEqual( 2, match.Groups[ 2 ].Index );

            Assert.AreEqual( "bb", match.Groups[ 3 ].Value );
            Assert.AreEqual( 3, match.Groups[ 3 ].Index );
        }

        [TestMethod( )]
        public void RegexNamedGroupsTest( ) {
            var match = new Regex( "(?<test>a+)" ).Match( "bbaaaa" );

            Assert.AreEqual( true, match.Success );
            Assert.AreEqual( 2, match.Groups.Count );
            Assert.AreEqual( "aaaa", match[ "test" ].Value );
            Assert.AreEqual( 1, match[ "test" ].Index );

            try {
                var rex = new Regex( "(?<test>a+)(?<test>b+" );
                Assert.Fail( "multiple named groups with the same name should not be supported" );
            }
            catch ( ParsingException ) { }
        }

        [TestMethod( )]
        public void RegexReplaceTest( ) {
            string input = "CamelCaseShallNotPass",
                result = "Camel_Case_Shall_Not_Pass";

            var rex = new Regex( "([a-z])([A-Z])" );

            Assert.AreEqual( result, rex.Replace( input, m => m[ 1 ] + "_" + m[ 2 ] ) );
            Assert.AreEqual( result, rex.Replace( input, "{1}_{2}" ) );
        }
    }
}
