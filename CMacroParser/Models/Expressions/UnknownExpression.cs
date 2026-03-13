using CMacroParser.Contracts;

namespace CMacroParser.Models.Expressions
{
    internal sealed class UnknownExpression : ExpressionBase
    {
        public required IExpression[] Expressions { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                foreach (var expression in Expressions)
                {
                    foreach (var token in expression.Tokens)
                    {
                        yield return token;
                    }
                }
            }
        }

        public override string Serialize(ISerializerOptions? options)
        {
            return string.Join(' ', Expressions.Select(x => x.Serialize(options)));
        }
    }
}
