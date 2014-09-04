using System;
using regexer;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace regexerTest {

    [TestClass]
    public class PositionTokenTest {

        [TestMethod]
        public void TestInputStartToken( ) {
            List<string> matches;

            matches = new Regex( @"^a" ).Matches( "aaaa" ).ToList( );
            Assert.AreEqual( 1, matches.Count );
            Assert.AreEqual( "a", matches.First( ) );

            matches = new Regex( @"a^a" ).Matches( "aaaa" ).ToList( );
            Assert.AreEqual( 0, matches.Count );

            matches = new Regex( @"\^a" ).Matches( "aaaa" ).ToList( );
            Assert.AreEqual( 0, matches.Count );

            matches = new Regex( @"\^a" ).Matches( "^aaaa" ).ToList( );
            Assert.AreEqual( 1, matches.Count );
            Assert.AreEqual( "^a", matches.First( ) );
        }


        [TestMethod]
        public void TestInputEndToken( ) {
            List<string> matches;

            matches = new Regex( @"a$" ).Matches( "aaaa" ).ToList( );
            Assert.AreEqual( 1, matches.Count );
            Assert.AreEqual( "a", matches.First( ) );

            matches = new Regex( @"a$a" ).Matches( "aa" ).ToList( );
            Assert.AreEqual( 0, matches.Count );

            matches = new Regex( @"\$a" ).Matches( "aaaa" ).ToList( );
            Assert.AreEqual( 0, matches.Count );

            matches = new Regex( @"\$a" ).Matches( "$aaaa" ).ToList( );
            Assert.AreEqual( 1, matches.Count );
            Assert.AreEqual( "$a", matches.First( ) );
        }
    }
}
