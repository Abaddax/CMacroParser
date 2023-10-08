using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Models.Expressions
{
    internal class CallExpression : ExpressionBase
    {
        public IdentifierToken Value { get; init; }
        public IExpression[] Arguments { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                yield return Value;
                yield return new PunctuatorToken() { Value = "(" };
                for (int i = 0; i < Arguments.Length; i++)
                {
                    foreach (var token in Arguments[i].Tokens)
                        yield return token;
                    if (i != Arguments.Length - 1)
                        yield return new PunctuatorToken() { Value = "," };
                }
                yield return new PunctuatorToken() { Value = ")" };
            }
        }

        public override string Serialize()
        {
            if (!Value.IsCall)
                throw new NotSupportedException();
            return $"{Value.Value}({string.Join(", ", Arguments.Select(a => a.Serialize()))})";
        }
    }
}
