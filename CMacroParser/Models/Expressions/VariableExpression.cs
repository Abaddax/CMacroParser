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

        public override bool ContainsUnknown(IEnumerable<IMacroDefinition> definitions)
        {
            var def = definitions.FirstOrDefault(x => x.Name == Value.Value && x.Args == null && x.Expression != null);
            if (def == null)
                return true;
            return def.Expression!.ContainsUnknown(definitions);
        }
        public override IExpression Expand(IEnumerable<IMacroDefinition> definitions)
        {
            var def = definitions.FirstOrDefault(x => x.Name == Value.Value && x.Args == null && x.Expression != null);
            if (def == null)
                return this;
            return def.Expression!.Expand(definitions);
        }
        public override string Serialize()
        {
            if (Value.IsCall)
                throw new NotSupportedException();
            return Value.Value;
        }
    }
}
