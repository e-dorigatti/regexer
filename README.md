regexer
=======

Regular expression engine written in C#.
You will need Visual Studio 2012 to open the soloution and [Doxygen](http://www.doxygen.org/index.html) to build the documentation. The solution contains a test project with simple tests as well.

Regular expression patterns are compiled into a tree-like structure used when the actual matching occurs; the compilation stage is located in [Tokenize.cs](/regexer/Tokenize.cs), while matching and backtracking are overridden methods in the tokens which constitute the syntax tree, [GroupToken](/regexer/GroupToken.cs), [LiteralToken](/regexer/LiteralToken.cs), [OrToken](/regexer/OrToken.cs) and [QuantifierToken](/regexer/QuantifierToken.cs).
