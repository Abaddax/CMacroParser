using CMacroParser.Contracts;

namespace CMacroParser.Tokenizer
{
    public static class Tokenizer
    {
        internal static readonly IReadOnlySet<char> Digits = new HashSet<char>("0123456789");
        internal static readonly IReadOnlySet<char> HexDigits = new HashSet<char>("abcdef");
        internal static readonly IReadOnlySet<char> Separators = new HashSet<char>(" \\\r\n\t");
        internal static readonly IReadOnlySet<char> NumberEnd = new HashSet<char>("ulfd");
        internal static readonly IReadOnlySet<char> Operators = new HashSet<char>("+-*/%<>=~!^|&?:");
        internal static readonly IReadOnlySet<char> Punctuators = new HashSet<char>(",;()[]{}");
        internal static readonly IReadOnlySet<string> Keywords = new HashSet<string>
        {
            "auto",
            "break",
            "case",
            "char",
            "const",
            "continue",
            "default",
            "do",
            "double",
            "else",
            "enum",
            "extern",
            "float",
            "for",
            "goto",
            "if",
            "int",
            "long",
            "register",
            "return",
            "short",
            "signed",
            "sizeof",
            "static",
            "struct",
            "switch",
            "typedef",
            "union",
            "unsigned",
            "void",
            "volatile",
            "while"
        };

        public static IEnumerable<IToken> Tokenize(this string expression)
        {
            int pos = 0;
            while (pos < expression.Length)
            {
                ReadOnlySpan<char> chars = expression[pos..];
                int skip = 1;
                if (chars.IsSeperator())
                    chars.SkipSeperator(out skip);
                else if (chars.IsPunctuator())
                    yield return chars.ReadPunctuator(out skip);
                else if (chars.IsKeyword())
                    yield return chars.ReadKeyword(out skip);
                else if (chars.IsOperator())
                    yield return chars.ReadOperator(out skip);
                else if (chars.IsLiteral())
                    yield return chars.ReadLiteral(out skip);
                else if (chars.IsComment())
                    ;
                else if (chars.IsIdentifier())
                    yield return chars.ReadIdentifier(out skip);
                else
                    throw new NotSupportedException();
                pos += skip;
            }
        }

        public static string Serialize(this ReadOnlySpan<IToken> tokens)
        {
            return tokens.ToArray().Serialize();
        }
        public static string Serialize(this IEnumerable<IToken> tokens)
        {
            if (tokens == null)
                return string.Empty;
            bool first = true;
            return string.Join(' ', tokens.Select(t => t.Value));
        }

    }
}
