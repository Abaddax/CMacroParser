using Abaddax.CMacroParser.Contracts;
using Abaddax.CMacroParser.Models.Tokens;

namespace Abaddax.CMacroParser.Models.Expressions
{
    /// <remarks>
    /// FUNC(A, B)
    /// </remarks>
    internal sealed class CallExpression : ExpressionBase
    {
        public required IdentifierToken Value { get; init; }
        public required IExpression[] Arguments { get; init; }

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

        public override string Serialize(ISerializerOptions? options)
        {
            if (!Value.IsCall)
                throw new NotSupportedException();
            return $"{Value.Value}({string.Join(", ", Arguments.Select(a => a.Serialize(options)))})";
        }
    }
}
