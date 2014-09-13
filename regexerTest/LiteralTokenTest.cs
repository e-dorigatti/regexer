using regexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace regexerTest {

    [TestClass( )]
    public class LiteralTokenTest {

        Dictionary<string, string> _goodPatterns = new Dictionary<string, string>( ) { 
            { "abc",        "abc"        }, {   "^abc",       "abc"                     },
            { "^a-fgh",     "abcdefgh"   }, {   "A-F",        "ABCDEF"                  },
            { "0-4",        "01234"      }, {   "^a-fA-F",    "abcdefABCDEF"            },
            { "ab-ef12-4",  "abcdef1234" }, {   "\\dabc-f",   "0123456789abcdef"        },
            { "\\d",        "0123456789" }, {   "\\s",        " \t"                     },
            { "\\w", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"  },
        };

        [TestMethod( )]
        public void LiteralTokenGoodPatternsTest( ) {
            foreach ( var kvp in _goodPatterns ) {
                var e = new LiteralToken( kvp.Key );
                Assert.AreEqual( kvp.Value, e.Pattern );

                if ( kvp.Key.StartsWith( "^" ) )
                    Assert.AreEqual( true, e.IsNegative );
                else Assert.AreEqual( false, e.IsNegative );
            }

        }

        [TestMethod( )]
        public void LiteralTokenBadPatternsTest( ) {
            var badPatterns = new List<string>( ) { 
                "ab-", "-ab", "AB-", "-AB", "12-", "-89",
                "z-a", "Z-A", "9-0",
                "a-9", "A-z", "A-9", "0-a", "0-A",
            };

            foreach ( string pattern in badPatterns ) {
                try {
                    var e = new LiteralToken( pattern );
                    Assert.Fail( "no exception thrown for pattern {0}, chars are {1}",
                        pattern, e.Pattern );
                }
                catch ( ParsingException ) { }
            }
        }

        [TestMethod( )]
        public void LiteralTokenMatchesTest( ) {
            foreach ( var kvp in _goodPatterns ) {
                var e = new LiteralToken( kvp.Key );

                // matches all the specified characters
                foreach ( char c in kvp.Value ) {
                    if ( kvp.Key.StartsWith( "^" ) )
                        Assert.IsFalse( e.Matches( c ) );
                    else Assert.IsTrue( e.Matches( c ) );
                }

                // matches ONLY the specified characters
                Assert.AreEqual( kvp.Value.Length, e.Pattern.Length );
            }
        }
    }
}