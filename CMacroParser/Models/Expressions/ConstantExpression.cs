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

        public override string Serialize()
        {
            return Value.Value;
        }
    }
}
