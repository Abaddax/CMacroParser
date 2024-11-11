using CMacroParser.Contracts;

namespace CMacroParser.UnitTest
{
    [TestFixture]
    public class Parser_Test
    {
        [Test]
        [Category("Basic")]
        #region [TestCases]
        [TestCase("123", "123")]
        [TestCase("(123)", "(123)")]
        [TestCase("((123))", "(123)")]
        [TestCase("-123", "-123")]
        [TestCase("(-123)", "(-123)")]
        [TestCase("A", "A")]
        [TestCase("(A)", "(A)")]
        [TestCase("((A))", "(A)")]
        [TestCase("-A", "-A")]
        [TestCase("(-A)", "(-A)")]
        [TestCase("FUNC(3e2, true)", "FUNC(300, true)")]
        [TestCase("-FUNC(3e2, true)", "-FUNC(300, true)")]
        [TestCase("FUNC(FUNC2(A))", "FUNC(FUNC2(A))")]
        [TestCase("(int)12.3", "(int)12.3")]
        [TestCase("(int)(12.3)", "(int)(12.3)")]
        [TestCase("(int)FUNC(A, B)", "(int)FUNC(A, B)")]
        [TestCase("++a", "++a")]
        [TestCase("a++", "a++")]
        [TestCase("1 + 3.2", "(1 + 3.2)")]
        [TestCase("(1) + (3.2)", "((1) + (3.2))")]
        [TestCase("(1 + 2) > 1", "((1 + 2) > 1)")]
        [TestCase("1 + 2 * 3", "(1 + (2 * 3))")]
        [TestCase("(1 + 2) * 3", "((1 + 2) * 3)")]
        [TestCase("1 * (2 + 3)", "(1 * (2 + 3))")]
        [TestCase("1 + 2 * 3 / 4", "(1 + ((2 * 3) / 4))")]
        [TestCase("1 * 2 + 3 * 4", "((1 * 2) + (3 * 4))")]
        [TestCase("(int)A + 2 * 3", "((int)A + (2 * 3))")]
        [TestCase("(int)A * 2 + 3", "(((int)A * 2) + 3)")]
        [TestCase("VAL ? 1 : 2", "(VAL ? 1 : 2)")]
        [TestCase("(VAL) ? (1) : (2)", "((VAL) ? (1) : (2))")]
        [TestCase("VAL ? (1 + 2) : (2 + 3)", "(VAL ? (1 + 2) : (2 + 3))")]
        [TestCase("VAL ? (1 + 2) : (2 + 3) + 1", "((VAL ? (1 + 2) : (2 + 3)) + 1)")]
        [TestCase("1 > 2", "(1 > 2)")]
        [TestCase("!(1 > 2)", "!(1 > 2)")]
        [TestCase("!(1 > FUNC(A))", "!(1 > FUNC(A))")]
        #endregion
        public void T1_ParseExpression(string input, string output)
        {
            var expression = Parser.Parser.ParseExpression(input);

            Assert.NotNull(expression);
            Assert.IsTrue(expression.Tokens.Any());

            Assert.AreEqual(output, expression.Serialize());
        }

        [Test]
        [Category("Basic")]
        #region [TestCases]
        [TestCase("A 2", LiteralType.@int)]
        [TestCase("#define B 3", LiteralType.@double)]
        [TestCase("FUNC(A) A * 2", LiteralType.@double)]
        #endregion
        public void T2_ParseDefinition(string input, LiteralType type)
        {
            var definition = Parser.Parser.ParseDefinition(input);

            Assert.NotNull(definition);
        }

        [Test]
        [Category("Basic")]
        #region [TestCases]
        [TestCase("123", LiteralType.@int)]
        [TestCase("0", LiteralType.@int)]
        [TestCase("-123", LiteralType.@int)]
        [TestCase("123u", LiteralType.@uint)]
        [TestCase("0u", LiteralType.@uint)]
        [TestCase("00u", LiteralType.@uint)]
        [TestCase("123U", LiteralType.@uint)]
        [TestCase("0U", LiteralType.@uint)]
        [TestCase("123l", LiteralType.@long)]
        [TestCase("0l", LiteralType.@long)]
        [TestCase("-123l", LiteralType.@long)]
        [TestCase("123L", LiteralType.@long)]
        [TestCase("0L", LiteralType.@long)]
        [TestCase("-123L", LiteralType.@long)]
        [TestCase("123ll", LiteralType.@long)]
        [TestCase("0ll", LiteralType.@long)]
        [TestCase("-123ll", LiteralType.@long)]
        [TestCase("123LL", LiteralType.@long)]
        [TestCase("0LL", LiteralType.@long)]
        [TestCase("-123LL", LiteralType.@long)]
        [TestCase("123ul", LiteralType.@ulong)]
        [TestCase("0ul", LiteralType.@ulong)]
        [TestCase("123UL", LiteralType.@ulong)]
        [TestCase("0UL", LiteralType.@ulong)]
        [TestCase("123ull", LiteralType.@ulong)]
        [TestCase("0ull", LiteralType.@ulong)]
        [TestCase("123ULL", LiteralType.@ulong)]
        [TestCase("0ULL", LiteralType.@ulong)]
        [TestCase("0f", LiteralType.@float)]
        [TestCase("1.23f", LiteralType.@float)]
        [TestCase("1.23F", LiteralType.@float)]
        [TestCase("0.0", LiteralType.@double)]
        [TestCase("1.23", LiteralType.@double)]
        [TestCase("0.0L", LiteralType.@decimal)]
        [TestCase("1.23L", LiteralType.@decimal)]
        [TestCase("1.23l", LiteralType.@decimal)]
        [TestCase("123 + 1.23f", LiteralType.@float)]
        [TestCase("123 + 1.23", LiteralType.@double)]
        [TestCase("123 / 2 + 1.23", LiteralType.@double)]
        [TestCase("(float)123", LiteralType.@float)]
        [TestCase("(float)0", LiteralType.@float)]
        [TestCase("(double)0", LiteralType.@double)]
        [TestCase("(long double)0", LiteralType.@decimal)]
        [TestCase("true ? 3 : 2e1", LiteralType.@double)]
        [TestCase("1 > 2", LiteralType.@bool)]
        [TestCase("!(1 > 2)", LiteralType.@bool)]
        [TestCase("2.3 != 3", LiteralType.@bool)]
        [TestCase("\"test\"", LiteralType.@string)]
        [TestCase("(int)((double)2 + 3)", LiteralType.@int)]
        #endregion
        public void T3_DeduceType(string input, LiteralType type)
        {
            var expression = Parser.Parser.ParseExpression(input);

            Assert.NotNull(expression);
            Assert.IsTrue(expression.Tokens.Any());

            var deducedType = expression.DeduceType();

            Assert.AreEqual(type, expression.DeduceType());
        }

        [Test]
        [Category("Basic")]
        #region [TestCases]
        [TestCase("123", true)]
        [TestCase("A", false)]
        [TestCase("A", true, "A 2")]
        [TestCase("FUNC(3e2, B)", false, "FUNC(a,b) a+b", "B A")]
        [TestCase("FUNC(3e2, B)", true, "FUNC(a,b) a+b", "B A", "A 2")]
        #endregion
        public void T4_IsConst(string input, bool isConst, params string[] definitions)
        {
            var defs = definitions.Select(x => Parser.Parser.ParseDefinition(x)).ToArray();
            var expr = Parser.Parser.ParseExpression(input);

            Assert.AreEqual(isConst, expr.IsConst(defs));
        }

        [Test]
        [Category("Expansion")]
        #region [TestCases]
        [TestCase("123", false)]
        [TestCase("A", true)]
        [TestCase("A", false, "A 2")]
        [TestCase("FUNC(3e2, B)", true, "FUNC(a,b) a+b", "B A")]
        [TestCase("FUNC(3e2, B)", false, "FUNC(a,b) a+b", "B A", "A 2")]
        #endregion
        public void T5_ContainsUnknown(string input, bool containsUnknown, params string[] definitions)
        {
            var defs = definitions.Select(x => Parser.Parser.ParseDefinition(x)).ToArray();
            var expr = Parser.Parser.ParseExpression(input);

            Assert.AreEqual(containsUnknown, expr.ContainsUnknown(defs));
        }

        [Test]
        [Category("Expansion")]
        #region [TestCases]
        [TestCase("123", "123")]
        [TestCase("A", "A")]
        [TestCase("A", "2", "A 2")]
        [TestCase("FUNC(3e2, B)", "(300 + A)", "FUNC(a,b) a+b", "B A")]
        [TestCase("FUNC(3e2, B)", "(300 + 2)", "FUNC(a,b) a+b", "B A", "A 2")]
        #endregion
        public void T6_Expand(string input, string output, params string[] definitions)
        {
            var defs = definitions.Select(x => Parser.Parser.ParseDefinition(x)).ToArray();
            var expr = Parser.Parser.ParseExpression(input);

            var expanded = expr.Expand(defs);

            Assert.NotNull(expanded);

            Assert.AreEqual(output, expanded.Serialize());
        }
    }
}
