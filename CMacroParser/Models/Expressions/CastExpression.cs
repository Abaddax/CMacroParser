using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;
using CMacroParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Expressions
{
    internal class CastExpression : ExpressionBase
    {
        public IExpression Value { get; init; }
        public IToken TargetType { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                yield return new PunctuatorToken() { Value = "(" };
                yield return TargetType;
                yield return new PunctuatorToken() { Value = ")" };
                yield return new PunctuatorToken() { Value = "(" };
                foreach (var token in Value.Tokens)
                    yield return token;
                yield return new PunctuatorToken() { Value = ")" };
            }
        }

        public override bool ContainsUnknown(IEnumerable<IMacroDefinition> definitions)
        {
            return Value.ContainsUnknown(definitions);
        }
        public override IExpression Expand(IEnumerable<IMacroDefinition> definitions)
        {
            return new CastExpression()
            {
                TargetType = TargetType,
                Value = Value.Expand(definitions),
            };
        }
        public override string Serialize()
        {
            return $"({this.DeduceType()}){Value.Serialize()}";
        }
    }
}
