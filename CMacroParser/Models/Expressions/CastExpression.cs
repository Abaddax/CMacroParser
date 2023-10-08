using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Models.Expressions
{
    internal class CastExpression : ExpressionBase
    {
        public IExpression Value { get; init; }
        public IToken TargetType { get; init; }

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

        public override string Serialize()
        {
            return $"({this.DeduceType()}){Value.Serialize()}";
        }
    }
}
