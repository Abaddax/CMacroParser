using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

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

        public override string Serialize(ISerializerOptions? options)
        {
            var ser = Expression.Serialize(options);
            if (ser.StartsWith('(') && ser.EndsWith(')'))
                return ser;
            return $"({ser})";
        }
    }
}
