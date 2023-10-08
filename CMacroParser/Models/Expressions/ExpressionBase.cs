using CMacroParser.Contracts;

namespace CMacroParser.Models.Expressions
{
    internal abstract class ExpressionBase : IExpression
    {
        public abstract IEnumerable<IToken> Tokens { get; }

        public abstract string Serialize();
        public override string ToString()
        {
            return Serialize();
        }
    }
}
