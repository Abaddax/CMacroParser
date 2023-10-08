using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Expressions
{
    internal class GroupExpression : ExpressionBase
    {
        public IExpression Expression { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                yield return new PunctuatorToken() { Value = "(" };
                foreach (var token in Expression.Tokens)
                    yield return token;
                yield return new PunctuatorToken() { Value = ")" };
            }
        }


        public override bool ContainsUnknown(IEnumerable<IMacroDefinition> definitions)
        {
            return Expression.ContainsUnknown(definitions);
        }
        public override IExpression Expand(IEnumerable<IMacroDefinition> definitions)
        {
            return new GroupExpression()
            {
                Expression = Expression.Expand(definitions)
            };
        }
        public override string Serialize()
        {
            var ser = Expression.Serialize();
            if (ser.StartsWith('(') && ser.EndsWith(')'))
                return ser;
            return $"({ser})";
        }
    }
}
