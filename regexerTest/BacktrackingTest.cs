using System;
using System.Linq;
using System.Collections.Generic;
using regexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace regexerTest {

    [TestClass]
    public class BacktrackingTest {

        [TestMethod]
        public void QuantifierTokenBacktrackingTest( ) {
            var patterns = new[ ] { 
                new { Pattern = "a+a+a+", Input = "aaaaa", Result = "aaaaa" },
                new { Pattern = "a+?a+a+", Input = "aaaaa", Result = "aaaaa" },
                new { Pattern = "ab.*?a", Input = "abbbba", Result = "abbbba" },
            };

            foreach ( var p in patterns ) {
                var rex = new Regex( p.Pattern );
                var matches = rex.Matches( p.Input )
                    .Select( m => m.Value ).ToList( );

                Assert.AreEqual( 1, matches.Count );
                Assert.AreEqual( p.Result, matches.First( ) );
            }
        }

        [TestMethod]
        public void GroupTokenBacktrackingTest( ) {
            // this assumes quantifier tokens backtrack correctly
            string pattern = @"(ab+)(ba+)(ab+)", input = "aaabbbaaabbbaaa";

            var rex = new Regex( pattern );
            var matches = rex.Matches( input )
                .Select( m => m.Value ).ToList( );

            Assert.AreEqual( 1, matches.Count );
            Assert.AreEqual( "abbbaaabbb", matches.First( ) );
        }
    }
}
