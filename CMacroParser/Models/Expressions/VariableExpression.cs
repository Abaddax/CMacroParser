using Abaddax.CMacroParser.Contracts;
using Abaddax.CMacroParser.Models.Tokens;

namespace Abaddax.CMacroParser.Models.Expressions
{
    /// <remarks>
    /// PI
    /// </remarks>
    internal sealed class VariableExpression : ExpressionBase
    {
        public required IdentifierToken Value { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                yield return Value;
            }
        }

        public override string Serialize(ISerializerOptions? options)
        {
            if (Value.IsCall)
                throw new NotSupportedException();
            return Value.Value;
        }
    }
}
