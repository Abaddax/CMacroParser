using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;
using CMacroParser.Tokenizer;
using System;
using System.Diagnostics;

namespace CMacroParser.UnitTest
{
    [TestFixture]
    public class Tokenizer_Test
    {
        private static IToken[] Tokenize(string expression) => expression.Tokenize().ToArray();
        private static void AssertAreEqual(TokenType type, string value, IToken actual)
        {
            Assert.AreEqual(type, actual.TokenType);
            Assert.AreEqual(value, actual.Value);
        }
        private static TToken AssertTokenType<TToken>(IToken actual)
        {
            Assert.IsTrue(actual is TToken);
            return (TToken)actual;
        }

        [Test]
        [Category("Basic")]
        #region [TestCases]
        [TestCase(",", ",")]
        [TestCase(";", ";")]
        [TestCase("(", "(")]
        [TestCase(")", ")")]
        [TestCase("[", "[")]
        [TestCase("]", "]")]
        [TestCase("{", "{")]
        [TestCase("}", "}")]
        [TestCase(",;()[]{}", ",", ";", "(", ")", "[", "]", "{", "}")]
        #endregion
        public void T1_Punctuators(string input, params string[] output)
        {
            var tokens = Tokenize(input);

            Assert.AreEqual(true, tokens.Any());
            Assert.AreEqual(output.Length, tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                AssertAreEqual(TokenType.Punctuator, output[i], tokens[i]);
                var token = AssertTokenType<PunctuatorToken>(tokens[i]);
            }
        }

        [Test]
        [Category("Basic")]
        #region [TestCases]
        [TestCase("auto", "auto")]
        [TestCase("break", "break")]
        [TestCase("case", "case")]
        [TestCase("char", "char")]
        [TestCase("const", "const")]
        [TestCase("continue", "continue")]
        [TestCase("default", "default")]
        [TestCase("do", "do")]
        [TestCase("double", "double")]
        [TestCase("else", "else")]
        [TestCase("enum", "enum")]
        [TestCase("extern", "extern")]
        [TestCase("float", "float")]
        [TestCase("for", "for")]
        [TestCase("goto", "goto")]
        [TestCase("if", "if")]
        [TestCase("int", "int")]
        [TestCase("long", "long")]
        [TestCase("register", "register")]
        [TestCase("return", "return")]
        [TestCase("short", "short")]
        [TestCase("signed", "signed")]
        [TestCase("sizeof", "sizeof")]
        [TestCase("static", "static")]
        [TestCase("struct", "struct")]
        [TestCase("switch", "switch")]
        [TestCase("typedef", "typedef")]
        [TestCase("union", "union")]
        [TestCase("unsigned", "unsigned")]
        [TestCase("void", "void")]
        [TestCase("volatile", "volatile")]
        [TestCase("while", "while")]
        #endregion
        public void T2_Keywords(string input, params string[] output)
        {
            var tokens = Tokenize(input);

            Assert.AreEqual(true, tokens.Any());
            Assert.AreEqual(output.Length, tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                AssertAreEqual(TokenType.Keyword, output[i], tokens[i]);
                var token = AssertTokenType<KeywordToken>(tokens[i]);
            }
        }

        [Test]
        [Category("Basic")]
        #region [TestCases]
        [TestCase("+", "+")]
        [TestCase("-", "-")]
        [TestCase("*", "*")]
        [TestCase("/", "/")]
        [TestCase("%", "%")]
        [TestCase("<", "<")]
        [TestCase(">", ">")]
        [TestCase("=", "=")]
        [TestCase("~", "~")]
        [TestCase("!", "!")]
        [TestCase("^", "^")]
        [TestCase("|", "|")]
        [TestCase("&", "&")]
        [TestCase("?", "?")]
        [TestCase(":", ":")]
        [TestCase("++", "++")]
        [TestCase("--", "--")]
        [TestCase("+=", "+=")]
        [TestCase("-=", "-=")]
        [TestCase("*=", "*=")]
        [TestCase("/=", "/=")]
        [TestCase("%=", "%=")]
        [TestCase("<<", "<<")]
        [TestCase(">>", ">>")]
        [TestCase("==", "==")]
        [TestCase(">=", ">=")]
        [TestCase("<=", "<=")]
        [TestCase(">>=", ">>=")]
        [TestCase("<<=", "<<=")]
        [TestCase("||", "||")]
        [TestCase("&&", "&&")]
        [TestCase("^=", "^=")]
        [TestCase("|=", "|=")]
        [TestCase("&=", "&=")]
        [TestCase("->", "->")]
        [TestCase("->*", "->*")]
        #endregion
        public void T3_Operators(string input, params string[] output)
        {
            var tokens = Tokenize(input);

            Assert.AreEqual(true, tokens.Any());
            Assert.AreEqual(output.Length, tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                AssertAreEqual(TokenType.Operator, output[i], tokens[i]);
                var token = AssertTokenType<OperatorToken>(tokens[i]);
            }
        }

        [Test]
        [Category("Basic")]
        #region [TestCases]
        #region Char
        [TestCase("'c'", LiteralType.@char, "'c'")]
        [TestCase("'\\c'", LiteralType.@char, "'\\c'")]
        #endregion
        #region String
        [TestCase("\"string\"", LiteralType.@string, "\"string\"")]
        [TestCase("\"stringwith\\\"quotes\\\"\"", LiteralType.@string, "\"stringwith\\\"quotes\\\"\"")]
        #endregion
        #region Double
        [TestCase(".123", LiteralType.@double, "0.123")]
        [TestCase("0.123", LiteralType.@double, "0.123")]
        [TestCase("1.23", LiteralType.@double, "1.23")]
        [TestCase("1e2", LiteralType.@double, "100")]
        [TestCase("1e-2", LiteralType.@double, "0.01")]
        [TestCase("1.1e2", LiteralType.@double, "110")]
        [TestCase("1.1e+2", LiteralType.@double, "110")]
        [TestCase("1.1e-2", LiteralType.@double, "0.011")]
        #endregion
        #region Float
        [TestCase(".123f", LiteralType.@float, "0.123")]
        [TestCase("0.123f", LiteralType.@float, "0.123")]
        [TestCase("1.23f", LiteralType.@float, "1.23")]
        [TestCase("1e2f", LiteralType.@float, "100")]
        [TestCase("1e+2f", LiteralType.@float, "100")]
        [TestCase("1e-2f", LiteralType.@float, "0.01")]
        [TestCase("1.1e2f", LiteralType.@float, "110")]
        [TestCase("1.1e-2f", LiteralType.@float, "0.011")]
        #endregion
        #region Decimal
        [TestCase(".123l", LiteralType.@decimal, "0.123")]
        [TestCase("0.123l", LiteralType.@decimal, "0.123")]
        [TestCase("1.23l", LiteralType.@decimal, "1.23")]
        [TestCase("1e2l", LiteralType.@decimal, "100")]
        [TestCase("1e+2l", LiteralType.@decimal, "100")]
        [TestCase("1e-2l", LiteralType.@decimal, "0.01")]
        [TestCase("1.1e2l", LiteralType.@decimal, "110")]
        [TestCase("1.1e-2l", LiteralType.@decimal, "0.011")]
        #endregion
        #region Int
        [TestCase("0", LiteralType.@int, "0")]
        [TestCase("123", LiteralType.@int, "123")]
        [TestCase("0xAB", LiteralType.@int, "171")]
        [TestCase("0x00000010", LiteralType.@int, "16")]
        [TestCase("0xfffffffe", LiteralType.@int, "-2")]
        [TestCase("0b0101", LiteralType.@int, "5")]
        [TestCase("012", LiteralType.@int, "10")]
        #endregion
        #region UInt
        [TestCase("123u", LiteralType.@uint, "123")]
        [TestCase("0xABu", LiteralType.@uint, "171")]
        [TestCase("0x00000010U", LiteralType.@uint, "16")]
        [TestCase("0b0101u", LiteralType.@uint, "5")]
        [TestCase("012u", LiteralType.@uint, "10")]
        #endregion
        #region Long
        [TestCase("123l", LiteralType.@long, "123")]
        [TestCase("0xABl", LiteralType.@long, "171")]
        [TestCase("0x00000010L", LiteralType.@long, "16")]
        [TestCase("0b0101l", LiteralType.@long, "5")]
        [TestCase("012l", LiteralType.@long, "10")]
        #endregion
        #region ULong
        [TestCase("123ul", LiteralType.@ulong, "123")]
        [TestCase("123ull", LiteralType.@ulong, "123")]
        [TestCase("0xABul", LiteralType.@ulong, "171")]
        [TestCase("0x00000010UL", LiteralType.@ulong, "16")]
        [TestCase("0b0101ul", LiteralType.@ulong, "5")]
        [TestCase("012ul", LiteralType.@ulong, "10")]
        #endregion
        #region Bool
        [TestCase("true", LiteralType.@bool, "true")]
        [TestCase("false", LiteralType.@bool, "false")]
        #endregion
        #endregion
        public void T4_Literals(string input, LiteralType literalType, params string[] output)
        {
            var tokens = Tokenize(input);

            Assert.AreEqual(true, tokens.Any());
            Assert.AreEqual(output.Length, tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                AssertAreEqual(TokenType.Literal, output[i], tokens[i]);
                var token = AssertTokenType<LiteralToken>(tokens[i]);

                Assert.AreEqual(token.OriginalContent, input);
                Assert.AreEqual(literalType, token.LiteralType);
            }
        }

        [Test]
        [Category("Basic")]
        #region [TestCases]
        [TestCase("INTMAX", "INTMAX")]
        [TestCase("int_min", "int_min")]
        [TestCase("func2", "func2")]
        [TestCase("FLOAT2INT", "FLOAT2INT")]
        #endregion
        public void T5_Identifiers(string input, params string[] output)
        {
            var tokens = Tokenize(input);

            Assert.AreEqual(true, tokens.Any());
            Assert.AreEqual(output.Length, tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                AssertAreEqual(TokenType.Identifier, output[i], tokens[i]);
                var token = AssertTokenType<IdentifierToken>(tokens[i]);
            }
        }

        [Test]
        [Category("Combined")]
        #region [TestCases]
        //@@ identifies functions
        [TestCase("SAFE_RELEASE(punk)", "@SAFE_RELEASE@", "(", "punk", ")")]
        [TestCase("TAG('A','B','C')", "@TAG@", "(", "'A'", ",", "'B'", ",", "'C'", ")")]
        [TestCase("MULTILINE(A,\n B)", "@MULTILINE@", "(", "A", ",", "B", ")")]
        [TestCase("SPECIAL_FLAG (1 << 2)", "SPECIAL_FLAG", "(", "1", "<<", "2", ")")]
        #endregion
        public void T6_Calls(string input, params string[] output)
        {
            var tokens = Tokenize(input);

            Assert.AreEqual(true, tokens.Any());
            Assert.AreEqual(output.Length, tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                var isCall = output[i].StartsWith("@") && output[i].EndsWith("@");
                output[i] = output[i].Replace("@", "");

                if (tokens[i] is IdentifierToken token)
                {
                    AssertAreEqual(TokenType.Identifier, output[i], tokens[i]);
                    Assert.AreEqual(isCall, token.IsCall);
                }
                else
                {
                    Assert.AreEqual(output[i], tokens[i].Value);
                }
            }
        }

        [Test]
        [Category("Combined")]
        #region [TestCases]
        [TestCase("1 << 2", "1", "<<", "2")]
        [TestCase("(1<<2)", "(", "1", "<<", "2", ")")]
        [TestCase("-1 >> -2", "-", "1", ">>", "-", "2")]
        [TestCase("(int)(1.2 + 3.4f)", "(", "int", ")", "(", "1.2", "+", "3.4", ")")]
        #endregion
        public void T7_Expressions(string input, params string[] output)
        {
            var tokens = Tokenize(input);

            Assert.AreEqual(true, tokens.Any());
            Assert.AreEqual(output.Length, tokens.Length);

            for (int i = 0; i < tokens.Length; i++)
            {
                Assert.AreEqual(output[i], tokens[i].Value);
            }
        }

    }
}