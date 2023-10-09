using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Models.Expressions
{
    internal class UnaryOperatorExpression : ExpressionBase
    {
        public bool IsSuffixOperator { get; init; }
        public OperatorToken Operator { get; init; }
        public IExpression Expression { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                if (!IsSuffixOperator)
                    yield return Operator;
                foreach (var token in Expression.Tokens)
                    yield return token;
                if (IsSuffixOperator)
                    yield return Operator;
            }
        }

        public override string Serialize(ISerializerOptions? options)
        {
            if (!IsSuffixOperator)
                return $"{Operator.Value}{Expression.Serialize(options)}";
            else
                return $"{Expression.Serialize(options)}{Operator.Value}";
        }
    }
    internal class BinaryOperatorExpression : ExpressionBase
    {
        public IExpression LeftExpression { get; init; }
        public OperatorToken Operator { get; init; }
        public IExpression RightExpression { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                foreach (var token in LeftExpression.Tokens)
                    yield return token;
                yield return Operator;
                foreach (var token in RightExpression.Tokens)
                    yield return token;
            }
        }

        public override string Serialize(ISerializerOptions? options)
        {
            return $"({LeftExpression.Serialize(options)} {Operator.Value} {RightExpression.Serialize(options)})";
        }
    }
    internal class TernaryOperatorExpression : ExpressionBase
    {
        public IExpression Condition { get; init; }
        public OperatorToken Operator1 { get; init; }
        public IExpression TrueExpression { get; init; }
        public OperatorToken Operator2 { get; init; }
        public IExpression FalseExpression { get; init; }

        public override IEnumerable<IToken> Tokens
        {
            get
            {
                foreach (var token in Condition.Tokens)
                    yield return token;
                yield return Operator1;
                foreach (var token in TrueExpression.Tokens)
                    yield return token;
                yield return Operator2;
                foreach (var token in FalseExpression.Tokens)
                    yield return token;
            }
        }

        public override string Serialize(ISerializerOptions? options)
        {
            return $"({Condition.Serialize(options)} {Operator1.Value} {TrueExpression.Serialize(options)} {Operator2.Value} {FalseExpression.Serialize(options)})";
        }
    }
}
