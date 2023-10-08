using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Models.Expressions
{
    internal class VariableExpression : ExpressionBase
    {
        public IdentifierToken Value { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                yield return Value;
            }
        }

        public override string Serialize()
        {
            if (Value.IsCall)
                throw new NotSupportedException();
            return Value.Value;
        }
    }
}
