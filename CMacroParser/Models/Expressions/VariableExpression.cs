using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override bool Expand(IEnumerable<IDefinition> definitions)
        {
            throw new NotImplementedException();
        }
        public override string Serialize()
        {
            if (Value.IsCall)
                throw new NotSupportedException();
            return Value.Value;
        }
    }
}
