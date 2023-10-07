using CMacroParser.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Expressions
{
    internal abstract class ExpressionBase : IExpression
    {
        public abstract IEnumerable<IToken> Tokens { get; }
        public abstract bool Expand(IEnumerable<IDefinition> definitions);
        public abstract string Serialize();

        public override string ToString()
        {
            return Serialize();
        }
    }
}
