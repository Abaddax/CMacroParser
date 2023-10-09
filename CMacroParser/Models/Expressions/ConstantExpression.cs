using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Models.Expressions
{
    internal sealed class ConstantExpression : ExpressionBase
    {
        public LiteralToken Value { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                yield return Value;
            }
        }

        public override string Serialize(ISerializerOptions? options)
        {
            options ??= ISerializerOptions.Default;
            return $"{Value.Value}{options.GetLiteralSuffix(Value.LiteralType)}";
        }
    }
}
