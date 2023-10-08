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

        public override bool ContainsUnknown(IEnumerable<IMacroDefinition> definitions)
        {
            return false;
        }
        public override IExpression Expand(IEnumerable<IMacroDefinition> definitions)
        {
            return this;
        }
        public override string Serialize()
        {
            return Value.Value;
        }
    }
}
