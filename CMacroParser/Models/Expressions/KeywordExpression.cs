using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Models.Expressions
{
    /// <remarks>
    /// const
    /// </remarks>
    internal sealed class KeywordExpression : ExpressionBase
    {
        public required KeywordToken Value { get; init; }

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
            return $"{options.GetKeyword(Value.Value)}";
        }
    }
}
