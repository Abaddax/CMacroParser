using Abaddax.CMacroParser.Contracts;

namespace Abaddax.CMacroParser.Models.Expressions
{
    internal abstract class ExpressionBase : IExpression
    {
        public abstract IEnumerable<IToken> Tokens { get; }

        public abstract string Serialize(ISerializerOptions? options);
        public override string ToString()
        {
            return Serialize(null);
        }
    }
}
