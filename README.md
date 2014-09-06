regexer
=======

Regular expression engine written in C#.
You will need Visual Studio 2012 to open the soloution and [Doxygen](http://www.doxygen.org/index.html) to build the documentation. The solution contains a test project with simple tests as well.

### Features
 - **Capturing Groups:** You can enclose specific parts of the pattern in round brackets, creating a capture group; when you match a string you will have a collection of the content of the capture groups along with other information such as the position of the first and last character. Groups can be optionally named with the following syntax: `(?<name>pattern)`, this allows groups to be retrieved by name as well as by index. Group indexing starts from 1, with 0 being the whole match. For more info see [RegexMatch](/regexer/RegexMatch.cs), [RegexGroup](/regexer/RegexGroup.cs) and [buildMatch()](/regexer/Regex.cs#L97).
 - **Match Replacement:** You can search for specific patterns in a string and replace them with other content; be it the value returned by a function or a specific string. In both cases, you will have access to the matched part of the string either as a parameter of the function or through a special syntax in the replacement string: `{n}`, where `n` is the 1-based index of the capture group (0 is the whole match). For more info see the two replacement overloads, [with a function](/regexer/Regex.cs#L121) and [with a string](/regexer/Regex.cs#L144).
 - **Convenience Wrappers** allowing a more flexible and comfortable usage; you will find [static methods](/regexer/Regex.cs#L159) in the Regex class following .NET's regular expression interface and [extension methods](/regexer/Extension.cs) on strings for added clarity and ease of use.
 - **Standard Syntax:** Pattern are specified using the standard syntax for regexes; the [or operator](/regexer/OrToken.cs) `|`, [quantifiers](/regexer/QuantifierToken.cs) `?`, `*`, `+`, and `{m,n}` with the usual shortcuts, [start of string](/regexer/InputStartToken.cs) `^` and [end of string](/regexer/InputEndToken.cs) `$`, [named groups](/regexer/GroupToken.cs) `(?<name>pattern)` and [character classes](/regexer/LiteralToken.cs) `\w`, `\W`, `\d`, `\D`, `\s`, `\S`, `[pattern]` and `[^pattern]`.


### Interesting points
The most interesting parts are the [compilation stage](/regexer/Tokenize.cs), where the pattern is first parsed into tokens then converted to a syntax tree and the matching logic itself, where the tree is traversed and "polimorphic recursion" (let's call it this way) shows all its power.

One of the most challenging aspects of regular expression matching is *backtracking*. Backtracking is necessary because greedy quantifiers are really *too* gredy and eat up too many characters of the input string, making the following parts of the pattern to fail matching; moreover, greedy quantifiers turn out to be really *too* lazy and can make the match fail as well.
For example, consider the string `aaaaaa` and the pattern `(a+)a`. The matching starts with the first capturing group, which matches the whole input; following, `a` fails to match because we are at the end of the string, so there is no character `a` to match. But this is obviously wrong, as the input actually matches the pattern! The solution is to ask the first group, `a+`, to kindly back off a little so that `a` can match the last character. Things get interesting for more complicated patterns, such as `a+a+a+a+`; try to think a situation where a lazy quantifier is being too lazy and screwing things up.
