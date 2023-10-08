using CMacroParser.Contracts;
using CMacroParser.Models.Definitions;
using CMacroParser.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Expressions
{
    internal class CallExpression : ExpressionBase
    {
        public IdentifierToken Value { get; init; }
        public IExpression[] Arguments { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                yield return Value;
                yield return new PunctuatorToken() { Value = "(" };
                for (int i = 0; i < Arguments.Length; i++)
                {
                    foreach (var token in Arguments[i].Tokens)
                        yield return token;
                    if (i != Arguments.Length - 1)
                        yield return new PunctuatorToken() { Value = "," };
                }
                yield return new PunctuatorToken() { Value = ")" };
            }
        }

        public override bool ContainsUnknown(IEnumerable<IMacroDefinition> definitions)
        {
            var functionDef = definitions.FirstOrDefault(x => x.Name == Value.Value && x.Args?.Length == Arguments.Length);
            if (functionDef == null)
                return true;
            foreach (var arg in Arguments)
            {
                if (arg.ContainsUnknown(definitions))
                    return true;
            }
            return false;
        }
        public override IExpression Expand(IEnumerable<IMacroDefinition> definitions)
        {
            var functionDef = definitions.FirstOrDefault(x => x.Name == Value.Value && x.Args?.Length == Arguments.Length && x.Expression != null);
            if (functionDef == null)
                return new CallExpression()
                {
                    Arguments = Arguments.Select(x => x.Expand(definitions)).ToArray(),
                    Value = Value
                };

            //Prepend arguments
            IEnumerable<IMacroDefinition> _definitions = definitions;
            foreach (var arg in Arguments.Select((arg, i) => (name: functionDef.Args![i], expr: arg)))
            {
                _definitions = _definitions.Prepend(new VariableDefinition()
                {
                    Name = arg.name,
                    Expression = arg.expr.Expand(definitions)
                });
            }
            return functionDef.Expression!.Expand(_definitions);
        }
        public override string Serialize()
        {
            if (!Value.IsCall)
                throw new NotSupportedException();
            return $"{Value.Value}({string.Join(", ", Arguments.Select(a => a.Serialize()))})";
        }
    }
}
