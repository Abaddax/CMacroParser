using Abaddax.CMacroParser.Contracts;
using Abaddax.CMacroParser.Models.Tokens;

namespace Abaddax.CMacroParser.Models.Expressions
{
    /// <remarks>
    /// 12.34f
    /// </remarks>
    internal sealed class ConstantExpression : ExpressionBase
    {
        public required LiteralToken Value { get; init; }

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
