using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public override bool Expand(IEnumerable<IDefinition> definitions)
        {
            return Expression.Expand(definitions);
        }
        public override string Serialize()
        {
            if (!IsSuffixOperator)
                return $"{Operator.Value}{Expression.Serialize()}";
            else
                return $"{Expression.Serialize()}{Operator.Value}";
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
        
        public override bool Expand(IEnumerable<IDefinition> definitions)
        {
            return LeftExpression.Expand(definitions) | RightExpression.Expand(definitions);
        }
        public override string Serialize()
        {
            return $"({LeftExpression.Serialize()} {Operator.Value} {RightExpression.Serialize()})";
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
        
        public override bool Expand(IEnumerable<IDefinition> definitions)
        {
            return Condition.Expand(definitions) | TrueExpression.Expand(definitions) | FalseExpression.Expand(definitions);
        }
        public override string Serialize()
        {
            return $"({Condition.Serialize()} {Operator1.Value} {TrueExpression.Serialize()} {Operator2.Value} {FalseExpression.Serialize()})";
        }
    }
}
