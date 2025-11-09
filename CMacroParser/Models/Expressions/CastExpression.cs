using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Models.Expressions
{
    internal sealed class CastExpression : ExpressionBase
    {
        public required IExpression Value { get; init; }
        public required IToken TargetType { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                yield return new PunctuatorToken() { Value = "(" };
                yield return TargetType;
                yield return new PunctuatorToken() { Value = ")" };
                yield return new PunctuatorToken() { Value = "(" };
                foreach (var token in Value.Tokens)
                    yield return token;
                yield return new PunctuatorToken() { Value = ")" };
            }
        }

        public override string Serialize(ISerializerOptions? options)
        {
            return $"({this.DeduceType()})({Value.Serialize(options)})";
        }
    }
}
