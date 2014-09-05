using regexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace regexerTest {

    [TestClass( )]
    public class TokenTest {

        [TestMethod( )]
        public void TokenizeOrTokenTest( ) {
            Token result = Token.Tokenize( "a|a|a|a" );
            Assert.IsInstanceOfType( result, typeof( GroupToken ) );

            GroupToken group = result as GroupToken;
            Assert.AreEqual(1, group.Content.Count);
            Assert.IsInstanceOfType( group.Content.First( ), typeof( OrToken ) );

            OrToken orToken = group.Content.First( ) as OrToken;
            Assert.AreEqual( 4, orToken.Alternatives.Count );

            foreach ( Token t in orToken.Alternatives ) {
                Assert.AreEqual( Token.TokenType.Literal, t.Type );
                Assert.AreEqual( "a", t.Text );
            }

            var badPatterns = new[ ] { "a|b|", "|a|b", "a||b" };
            foreach ( var pattern in badPatterns ) {
                try {
                    Token t = Token.Tokenize( pattern );
                    Assert.Fail( "no exception thrown for pattern: " + pattern );
                }
                catch ( ParsingException ) { }
            }
        }

        [TestMethod( )]
        public void TokenizeQuantifierTest( ) {
            var patterns = new[ ] {
                new { Pattern = "ba?",      MinOccurrences = 0, MaxOccurrences = 1,            Lazy = false },
                new { Pattern = "ba+",      MinOccurrences = 1, MaxOccurrences = int.MaxValue, Lazy = false },
                new { Pattern = "ba+?",     MinOccurrences = 1, MaxOccurrences = int.MaxValue, Lazy = true  },
                new { Pattern = "ba*",      MinOccurrences = 0, MaxOccurrences = int.MaxValue, Lazy = false },
                new { Pattern = "ba*?",     MinOccurrences = 0, MaxOccurrences = int.MaxValue, Lazy = true  },
                new { Pattern = "ba{1,2}",  MinOccurrences = 1, MaxOccurrences = 2,            Lazy = false },
                new { Pattern = "ba{,2}",   MinOccurrences = 0, MaxOccurrences = 2,            Lazy = false },
                new { Pattern = "ba{1,}",   MinOccurrences = 1, MaxOccurrences = int.MaxValue, Lazy = false },
            };

            foreach ( var p in patterns ) {
                Token root = Token.Tokenize( p.Pattern );
                Assert.IsInstanceOfType( root, typeof( GroupToken ) );

                Token first = ( root as GroupToken ).Content[ 0 ];
                Assert.AreEqual( Token.TokenType.Literal, first.Type );
                Assert.AreEqual( "b", first.Text );

                Token result = ( root as GroupToken ).Content[ 1 ];
                Assert.IsInstanceOfType( result, typeof( QuantifierToken ) );
                QuantifierToken quantifier = result as QuantifierToken;

                Assert.AreEqual( Token.TokenType.Literal, quantifier.Target.Type );
                Assert.AreEqual( "a", quantifier.Target.Text );

                Assert.AreEqual( quantifier.MinOccurrences, p.MinOccurrences,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.MinOccurrences, quantifier.MinOccurrences ) );

                Assert.AreEqual( quantifier.MaxOccurrences, p.MaxOccurrences,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.MaxOccurrences, quantifier.MaxOccurrences ) );

                Assert.AreEqual( quantifier.IsLazy, p.Lazy,
                    string.Format( "pattern: {0}, expected: {1}, actual: {2}",
                    p.Pattern, p.Lazy, quantifier.IsLazy ) );
            }
        }
    }
}

