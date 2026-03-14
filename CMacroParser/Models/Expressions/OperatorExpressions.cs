using Abaddax.CMacroParser.Contracts;
using Abaddax.CMacroParser.Models.Tokens;

namespace Abaddax.CMacroParser.Models.Expressions
{
    /// <remarks>
    /// i++
    /// </remarks>
    internal sealed class UnaryOperatorExpression : ExpressionBase
    {
        public required bool IsSuffixOperator { get; init; }
        public required OperatorToken Operator { get; init; }
        public required IExpression Expression { get; init; }

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
    /// <remarks>
    /// i * 2
    /// </remarks>
    internal class BinaryOperatorExpression : ExpressionBase
    {
        public required IExpression LeftExpression { get; init; }
        public required OperatorToken Operator { get; init; }
        public required IExpression RightExpression { get; init; }

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
    /// <remarks>
    /// x ? 1 : 0
    /// </remarks>
    internal class TernaryOperatorExpression : ExpressionBase
    {
        public required IExpression Condition { get; init; }
        public required OperatorToken Operator1 { get; init; }
        public required IExpression TrueExpression { get; init; }
        public required OperatorToken Operator2 { get; init; }
        public required IExpression FalseExpression { get; init; }

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
