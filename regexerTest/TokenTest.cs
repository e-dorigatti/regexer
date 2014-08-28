using regexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace regexerTest {

    [TestClass( )]
    public class TokenTest {

        [TestMethod( )]
        public void TokenizeOrTokenTest( ) {
            Token result = Token.Tokenize( "a|a|a|a" );
            Assert.IsInstanceOfType( result, typeof( OrToken ) );

            OrToken orToken = result as OrToken;
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

        [TestMethod( )]
        public void TokenizeOperatorPriorityTest( ) {
            Token root = Token.Tokenize( "(a|bc*)+|x" );

            Assert.IsInstanceOfType( root, typeof( OrToken ) );
            OrToken orRoot = root as OrToken;
            Assert.AreEqual( Token.TokenType.Literal, orRoot.Alternatives[ 1 ].Type );
            Assert.AreEqual( "x", orRoot.Alternatives[ 1 ].Text );

            Assert.IsInstanceOfType( orRoot.Alternatives[ 0 ], typeof( QuantifierToken ) );
            QuantifierToken quantifier = orRoot.Alternatives[ 0 ] as QuantifierToken;
            Assert.AreEqual( 1, quantifier.MinOccurrences );
            Assert.AreEqual( int.MaxValue, quantifier.MaxOccurrences );

            Assert.IsInstanceOfType( quantifier.Target, typeof( OrToken ) );
            OrToken innerOr = quantifier.Target as OrToken;
            Assert.AreEqual( Token.TokenType.Literal, innerOr.Alternatives[ 0 ].Type );
            Assert.AreEqual( "a", innerOr.Alternatives[ 0 ].Text );

            Assert.IsInstanceOfType( innerOr.Alternatives[ 1 ], typeof( GroupToken ) );
            GroupToken innerGroup = innerOr.Alternatives[ 1 ] as GroupToken;
            Assert.AreEqual( Token.TokenType.Literal, innerGroup.Content[ 0 ].Type );
            Assert.AreEqual( "b", innerGroup.Content[ 0 ].Text );

            Assert.IsInstanceOfType( innerGroup.Content[ 1 ], typeof( QuantifierToken ) );
            QuantifierToken innerQuantifier = innerGroup.Content[ 1 ] as QuantifierToken;
            Assert.AreEqual( 0, innerQuantifier.MinOccurrences );
            Assert.AreEqual( int.MaxValue, innerQuantifier.MaxOccurrences );
            Assert.AreEqual( Token.TokenType.Literal, innerQuantifier.Target.Type );
            Assert.AreEqual( "c", innerQuantifier.Target.Text );
        }
    }
}

