using regexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

/*
namespace regexerTest {

    [TestClass( )]
    public class QuantifierNodeTest {
        public TestContext TestContext { get; set; }

        [TestMethod( )]
        public void QuantifierConstructorTest( ) {
            var goodPatterns = new[ ] {
                new { Pattern = "?",   MinOccurrences = 0, MaxOccurrences = 1,            Lazy = false },
                new { Pattern = "+",   MinOccurrences = 1, MaxOccurrences = int.MaxValue, Lazy = false },
                new { Pattern = "+?",  MinOccurrences = 1, MaxOccurrences = int.MaxValue, Lazy = true  },
                new { Pattern = "*",   MinOccurrences = 0, MaxOccurrences = int.MaxValue, Lazy = false },
                new { Pattern = "*?",  MinOccurrences = 0, MaxOccurrences = int.MaxValue, Lazy = true  },
                new { Pattern = "1,2", MinOccurrences = 1, MaxOccurrences = 2,            Lazy = false },
                new { Pattern = ",2",  MinOccurrences = 0, MaxOccurrences = 2,            Lazy = false },
                new { Pattern = "1,",  MinOccurrences = 1, MaxOccurrences = int.MaxValue, Lazy = false },
            };

            foreach ( var p in goodPatterns ) {
                var node = new QuantifierToken( p.Pattern, null );

                Assert.AreEqual( node.MinOccurrences, p.MinOccurrences,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.MinOccurrences, node.MinOccurrences ) );

                Assert.AreEqual( node.MaxOccurrences, p.MaxOccurrences,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.MaxOccurrences, node.MaxOccurrences ) );

                Assert.AreEqual( node.Lazy, p.Lazy,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.Lazy, node.Lazy ) );

            }
        }
    }
}
*/
