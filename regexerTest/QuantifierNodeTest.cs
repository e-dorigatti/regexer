using regexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace regexerTest {

    [TestClass( )]
    public class QuantifierNodeTest {
        public TestContext TestContext { get; set; }

        [TestMethod( )]
        public void QuantifierConstructorTest( ) {
            var goodPatterns = new[ ] {
                new { Pattern = "?",     MinOccurrences = 0, MaxOccurrences = 1,            IsLazy = false },
                new { Pattern = "+",     MinOccurrences = 1, MaxOccurrences = int.MaxValue, IsLazy = false },
                new { Pattern = "+?",    MinOccurrences = 1, MaxOccurrences = int.MaxValue, IsLazy = true  },
                new { Pattern = "*",     MinOccurrences = 0, MaxOccurrences = int.MaxValue, IsLazy = false },
                new { Pattern = "*?",    MinOccurrences = 0, MaxOccurrences = int.MaxValue, IsLazy = true  },
                new { Pattern = "{1,2}", MinOccurrences = 1, MaxOccurrences = 2,            IsLazy = false },
                new { Pattern = "{,2}",  MinOccurrences = 0, MaxOccurrences = 2,            IsLazy = false },
                new { Pattern = "{1,}",  MinOccurrences = 1, MaxOccurrences = int.MaxValue, IsLazy = false },
                new { Pattern = "{3}",   MinOccurrences = 3, MaxOccurrences = 3,            IsLazy = false },
            };

            foreach ( var p in goodPatterns ) {
                var node = new QuantifierToken( p.Pattern, null );

                Assert.AreEqual( node.MinOccurrences, p.MinOccurrences,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.MinOccurrences, node.MinOccurrences ) );

                Assert.AreEqual( node.MaxOccurrences, p.MaxOccurrences,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.MaxOccurrences, node.MaxOccurrences ) );

                Assert.AreEqual( node.IsLazy, p.IsLazy,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.IsLazy, node.IsLazy ) );

            }
        }
    }
}
