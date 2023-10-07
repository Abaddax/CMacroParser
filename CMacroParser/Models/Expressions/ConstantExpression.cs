using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public override bool Expand(IEnumerable<IDefinition> definitions)
        {
            return false; //Final
        }
        public override string Serialize()
        {
            return Value.Value;
        }
    }
}
