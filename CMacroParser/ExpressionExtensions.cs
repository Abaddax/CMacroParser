using CMacroParser.Contracts;
using CMacroParser.Models.Definitions;
using CMacroParser.Models.Expressions;
using static CMacroParser.Parser.Parser;

namespace CMacroParser
{
    public static class ExpressionExtensions
    {
        #region DeduceType
        /// <summary>
        /// Try to deduce the type of the expression
        /// </summary>
        public static LiteralType DeduceType(this IExpression expression)
        {
            return expression switch
            {
                GroupExpression e => DeduceType(e.Expression),
                CallExpression e => LiteralType.unknown,
                CastExpression e => DeduceType(e),
                ConstantExpression e => e.Value.LiteralType,
                VariableExpression e => LiteralType.unknown,
                UnaryOperatorExpression e => DeduceType(e.Expression),
                BinaryOperatorExpression e => DeduceType(e),
                TernaryOperatorExpression e => DeduceType(e),
                _ => throw new NotSupportedException()
            };
        }

        private static LiteralType DeduceType(CastExpression expression)
        {
            //https://learn.microsoft.com/en-us/cpp/cpp/fundamental-types-cpp?view=msvc-170
            return expression.TargetType.Value.ToLowerInvariant() switch
            {
                "void" => LiteralType.@void,

                "bool" => LiteralType.@bool,

                "float" => LiteralType.@float,
                "double" => LiteralType.@double,
                "long double" => LiteralType.@decimal,

                "short" or
                "short int" or
                "signed short" or
                "signed short int" or
                "int16_t" or
                "__int16" => LiteralType.@short,
                "unsigned short" or
                "unsigned short int" or
                "uint16_t" or
                "unsigned  __int16" => LiteralType.@ushort,
                "int" or
                "signed" or
                "signed int" or
                "int32_t" or
                "__int32" => LiteralType.@int,
                "unsigned" or
                "unsigned int" or
                "uint32_t" or
                "unsigned __int32" => LiteralType.@uint,
                "long" or
                "long int" or
                "signed long" or
                "signed long int" or
                "long long" or
                "long long int" or
                "signed long long" or
                "signed long long int" or
                "int64_t" or
                "__int64" => LiteralType.@long,
                "unsigned long" or
                "unsigned long int" or
                "unsigned long long" or
                "unsigned long long int" or
                "uint64_t" or
                "unsigned __int64" => LiteralType.@ulong,

                "byte" or
                "int8_t" or
                "uint8_t" or
                "__int8" => LiteralType.@byte,

                "char" or
                "signed char" or
                "unsigned char" or
                "wchar_t" or
                "__wchar_t" => LiteralType.@char,

                _ => LiteralType.unknown
            };
        }
        private static LiteralType DeduceType(BinaryOperatorExpression expression)
        {
            OperationPrecedence.TryGetValue(expression.Operator.Value, out var precedence);
            if (precedence == 9 || precedence == 10) //Binary operators
                return LiteralType.@bool;

            var leftType = DeduceType(expression.LeftExpression);
            var rightType = DeduceType(expression.RightExpression);
            TypePrecedence.TryGetValue(leftType, out var leftPrecedence);
            TypePrecedence.TryGetValue(rightType, out var rightPrecedence);

            if (leftPrecedence < rightPrecedence)
                return leftType;
            else if (leftPrecedence == rightPrecedence && leftType != rightType)
                throw new Exception($"Unable to deduce type of {leftType} and {rightType}.");
            else
                return rightType;
        }
        private static LiteralType DeduceType(TernaryOperatorExpression expression)
        {
            var trueType = DeduceType(expression.TrueExpression);
            var falseType = DeduceType(expression.FalseExpression);
            TypePrecedence.TryGetValue(trueType, out var truePrecedence);
            TypePrecedence.TryGetValue(falseType, out var falsePrecedence);

            if (falsePrecedence < truePrecedence)
                return falseType;
            else
                return trueType;
        }
        #endregion

        #region ContainsUnknown
        /// <summary>
        /// Check if any unkown expressions exit within this expression
        /// </summary>
        public static bool ContainsUnknown(this IExpression expression, IEnumerable<IMacroDefinition> definitions)
        {
            return expression switch
            {
                GroupExpression e => ContainsUnknown(e.Expression, definitions),
                CallExpression e => ContainsUnknown(e, definitions),
                CastExpression e => ContainsUnknown(e.Value, definitions),
                ConstantExpression e => false,
                VariableExpression e => ContainsUnknown(e, definitions),
                UnaryOperatorExpression e => ContainsUnknown(e.Expression, definitions),
                BinaryOperatorExpression e => ContainsUnknown(e.LeftExpression, definitions) || ContainsUnknown(e.RightExpression, definitions),
                TernaryOperatorExpression e => ContainsUnknown(e.Condition, definitions) || ContainsUnknown(e.TrueExpression, definitions) || ContainsUnknown(e.FalseExpression, definitions),
                _ => throw new NotSupportedException()
            };
        }

        private static bool ContainsUnknown(CallExpression expression, IEnumerable<IMacroDefinition> definitions)
        {
            var functionDef = definitions.FirstOrDefault(x => x.Name == expression.Value.Value && x.Args?.Length == expression.Arguments.Length);
            if (functionDef == null)
                return true;
            foreach (var arg in expression.Arguments)
            {
                if (arg.ContainsUnknown(definitions))
                    return true;
            }
            return false;
        }
        private static bool ContainsUnknown(VariableExpression expression, IEnumerable<IMacroDefinition> definitions)
        {
            var def = definitions.FirstOrDefault(x => x.Name == expression.Value.Value && x.Args == null && x.Expression != null);
            if (def == null)
                return true;
            return def.Expression!.ContainsUnknown(definitions);
        }
        #endregion

        #region Expand
        /// <summary>
        /// Try to resolve unknown expressions with known macros from <paramref name="definitions"/>
        /// </summary>
        /// <returns>Expression with known expressions from <paramref name="definitions"/></returns>
        public static IExpression Expand(this IExpression expression, IEnumerable<IMacroDefinition> definitions)
        {
            return expression switch
            {
                GroupExpression e => new GroupExpression()
                {
                    Expression = e.Expression.Expand(definitions)
                },
                CallExpression e => Expand(e, definitions),
                CastExpression e => new CastExpression()
                {
                    TargetType = e.TargetType,
                    Value = e.Value.Expand(definitions),
                },
                ConstantExpression e => e,
                VariableExpression e => Expand(e, definitions),
                UnaryOperatorExpression e => new UnaryOperatorExpression()
                {
                    IsSuffixOperator = e.IsSuffixOperator,
                    Operator = e.Operator,
                    Expression = e.Expression.Expand(definitions),
                },
                BinaryOperatorExpression e => new BinaryOperatorExpression()
                {
                    LeftExpression = e.LeftExpression.Expand(definitions),
                    Operator = e.Operator,
                    RightExpression = e.RightExpression.Expand(definitions),
                },
                TernaryOperatorExpression e => new TernaryOperatorExpression()
                {
                    Condition = e.Condition.Expand(definitions),
                    Operator1 = e.Operator1,
                    TrueExpression = e.TrueExpression.Expand(definitions),
                    Operator2 = e.Operator2,
                    FalseExpression = e.FalseExpression.Expand(definitions)
                },
                _ => throw new NotSupportedException()
            };
        }

        private static IExpression Expand(CallExpression expression, IEnumerable<IMacroDefinition> definitions)
        {
            var functionDef = definitions.FirstOrDefault(x => x.Name == expression.Value.Value && x.Args?.Length == expression.Arguments.Length && x.Expression != null);
            if (functionDef == null)
                return new CallExpression()
                {
                    Arguments = expression.Arguments.Select(x => x.Expand(definitions)).ToArray(),
                    Value = expression.Value
                };

            //Prepend arguments
            IEnumerable<IMacroDefinition> _definitions = definitions;
            foreach (var arg in expression.Arguments.Select((arg, i) => (name: functionDef.Args![i], expr: arg)))
            {
                _definitions = _definitions.Prepend(new VariableDefinition()
                {
                    Name = arg.name,
                    Expression = arg.expr.Expand(definitions)
                });
            }
            return functionDef.Expression!.Expand(_definitions);
        }
        private static IExpression Expand(VariableExpression expression, IEnumerable<IMacroDefinition> definitions)
        {
            var def = definitions.FirstOrDefault(x => x.Name == expression.Value.Value && x.Args == null && x.Expression != null);
            if (def == null)
                return expression;
            return def.Expression!.Expand(definitions);
        }
        #endregion

        #region IsConst
        /// <summary>
        /// Check if expression in a compile-time-const
        /// </summary>
        public static bool IsConst(this IExpression expression, IEnumerable<IMacroDefinition> definitions = null)
        {
            definitions ??= Enumerable.Empty<IMacroDefinition>();
            expression = expression.Expand(definitions);
            return expression switch
            {
                GroupExpression e => IsConst(e, definitions),
                CallExpression e => false,
                CastExpression e => IsConst(e.Value, definitions),
                ConstantExpression e => true,
                VariableExpression e => false,
                UnaryOperatorExpression e => IsConst(e.Expression, definitions),
                BinaryOperatorExpression e => IsConst(e.LeftExpression, definitions) && IsConst(e.RightExpression, definitions),
                TernaryOperatorExpression e => IsConst(e.Condition, definitions) && IsConst(e.TrueExpression, definitions) | IsConst(e.FalseExpression, definitions),
                _ => throw new NotSupportedException()
            };
        }
        #endregion
    }
}
