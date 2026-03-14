using Abaddax.CMacroParser.Contracts;
using Abaddax.CMacroParser.Models.Tokens;

namespace Abaddax.CMacroParser.Models.Expressions
{
    /// <remarks>
    /// (int)(PI)
    /// </remarks>
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
            var value = Value.Serialize(options);
            if (value.StartsWith('(') && value.EndsWith(')'))
                return $"({this.DeduceLiteralType()}){value}";
            else
                return $"({this.DeduceLiteralType()})({value})";
        }
    }
}
