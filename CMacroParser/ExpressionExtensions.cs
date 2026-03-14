using Abaddax.CMacroParser.Contracts;
using Abaddax.CMacroParser.Models.Definitions;
using Abaddax.CMacroParser.Models.Expressions;
using static Abaddax.CMacroParser.Parser.Parser;

namespace Abaddax.CMacroParser
{
    public static class ExpressionExtensions
    {
        #region DeduceLiteralType
        /// <summary>
        /// Try to deduce the type of the literal/expression
        /// </summary>
        public static IDeducedType DeduceLiteralType(this IExpression expression)
        {
            return expression switch
            {
                GroupExpression e => DeduceLiteralType(e.Expression),
                CallExpression => IDeducedType.Create(string.Empty, LiteralType.unknown),
                CastExpression e => DeduceLiteralType(e),
                ConstantExpression e => IDeducedType.Create(e.Value.LiteralType.ToString(), e.Value.LiteralType),
                KeywordExpression e => DeduceLiteralType(e),
                VariableExpression => IDeducedType.Create(string.Empty, LiteralType.unknown),
                UnaryOperatorExpression e => DeduceLiteralType(e.Expression),
                BinaryOperatorExpression e => DeduceLiteralType(e),
                TernaryOperatorExpression e => DeduceLiteralType(e),
                UnknownExpression => IDeducedType.Create(string.Empty, LiteralType.unknown),
                _ => throw new NotSupportedException()
            };
        }

        private static IDeducedType DeduceLiteralType(CastExpression expression)
        {
            //https://learn.microsoft.com/en-us/cpp/cpp/fundamental-types-cpp?view=msvc-170
            LiteralType type = expression.TargetType.Value.ToLowerInvariant() switch
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

                _ => LiteralType.custom
            };
            return IDeducedType.Create(expression.TargetType.Value, type);
        }
        private static IDeducedType DeduceLiteralType(KeywordExpression expression)
        {
            if (!Enum.TryParse<LiteralType>(expression.Value.Value, out var literalType))
                return IDeducedType.Create(string.Empty, LiteralType.unknown);
            return IDeducedType.Create(literalType.ToString(), literalType);
        }
        private static IDeducedType DeduceLiteralType(BinaryOperatorExpression expression)
        {
            _ = _OperationPrecedence.TryGetValue(expression.Operator.Value, out var precedence);
            if (precedence == 9 || precedence == 10) //Binary operators
                return IDeducedType.Create("bool", LiteralType.@bool);

            var leftType = DeduceLiteralType(expression.LeftExpression);
            var rightType = DeduceLiteralType(expression.RightExpression);
            _ = _TypePrecedence.TryGetValue(leftType.Deduced, out var leftPrecedence);
            _ = _TypePrecedence.TryGetValue(rightType.Deduced, out var rightPrecedence);

            if (leftPrecedence < rightPrecedence)
                return leftType;
            else if (leftPrecedence == rightPrecedence && leftType.Deduced != rightType.Deduced)
                throw new Exception($"Unable to deduce type of {leftType.Deduced} and {rightType.Deduced}.");
            else
                return rightType;
        }
        private static IDeducedType DeduceLiteralType(TernaryOperatorExpression expression)
        {
            var trueType = DeduceLiteralType(expression.TrueExpression);
            var falseType = DeduceLiteralType(expression.FalseExpression);
            _ = _TypePrecedence.TryGetValue(trueType.Deduced, out var truePrecedence);
            _ = _TypePrecedence.TryGetValue(falseType.Deduced, out var falsePrecedence);

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
            expression = expression.Expand(definitions);
            return expression switch
            {
                GroupExpression e => ContainsUnknown(e.Expression, definitions),
                CallExpression e => ContainsUnknown(e, definitions),
                CastExpression e => ContainsUnknown(e.Value, definitions),
                ConstantExpression => false,
                KeywordExpression => false,
                VariableExpression e => ContainsUnknown(e, definitions),
                UnaryOperatorExpression e => ContainsUnknown(e.Expression, definitions),
                BinaryOperatorExpression e => ContainsUnknown(e.LeftExpression, definitions) || ContainsUnknown(e.RightExpression, definitions),
                TernaryOperatorExpression e => ContainsUnknown(e.Condition, definitions) || ContainsUnknown(e.TrueExpression, definitions) || ContainsUnknown(e.FalseExpression, definitions),
                UnknownExpression => true,
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
            var referencedExpressions = new HashSet<IExpression>();
            return expression.Expand(definitions, referencedExpressions);
        }

        private static IExpression Expand(this IExpression expression, IEnumerable<IMacroDefinition> definitions, ISet<IExpression> referencedExpressions)
        {
            //Add itself to tree branch
            if (!referencedExpressions.Add(expression))
                throw new Exception("Self referencing loop detected!");
            var expanded = expression switch
            {
                GroupExpression e => new GroupExpression()
                {
                    Expression = e.Expression.Expand(definitions, referencedExpressions)
                },
                CallExpression e => Expand(e, definitions, referencedExpressions),
                CastExpression e => new CastExpression()
                {
                    TargetType = e.TargetType,
                    Value = e.Value.Expand(definitions, referencedExpressions),
                },
                ConstantExpression e => e,
                KeywordExpression e => e,
                VariableExpression e => Expand(e, definitions, referencedExpressions),
                UnaryOperatorExpression e => new UnaryOperatorExpression()
                {
                    IsSuffixOperator = e.IsSuffixOperator,
                    Operator = e.Operator,
                    Expression = e.Expression.Expand(definitions, referencedExpressions),
                },
                BinaryOperatorExpression e => new BinaryOperatorExpression()
                {
                    LeftExpression = e.LeftExpression.Expand(definitions, referencedExpressions),
                    Operator = e.Operator,
                    RightExpression = e.RightExpression.Expand(definitions, referencedExpressions),
                },
                TernaryOperatorExpression e => new TernaryOperatorExpression()
                {
                    Condition = e.Condition.Expand(definitions, referencedExpressions),
                    Operator1 = e.Operator1,
                    TrueExpression = e.TrueExpression.Expand(definitions, referencedExpressions),
                    Operator2 = e.Operator2,
                    FalseExpression = e.FalseExpression.Expand(definitions, referencedExpressions)
                },
                UnknownExpression e => Expand(e, definitions, referencedExpressions),
                _ => throw new NotSupportedException()
            };
            //Tree branch finished ... remove self
            referencedExpressions.Remove(expression);
            return expanded;
        }
        private static IExpression Expand(CallExpression expression, IEnumerable<IMacroDefinition> definitions, ISet<IExpression> referencedExpressions)
        {
            var functionDef = definitions.FirstOrDefault(x => x.Name == expression.Value.Value && x.Args?.Length == expression.Arguments.Length && x.Expression != null);
            if (functionDef == null)
            {
                return new CallExpression()
                {
                    Arguments = expression.Arguments.Select(x => x.Expand(definitions, referencedExpressions)).ToArray(),
                    Value = expression.Value
                };
            }

            //Prepend arguments
            IEnumerable<IMacroDefinition> currentDefinitions = definitions;
            foreach (var arg in expression.Arguments.Select((arg, i) => (name: functionDef.Args![i], expr: arg)))
            {
                currentDefinitions = currentDefinitions.Prepend(new VariableDefinition()
                {
                    Name = arg.name,
                    Expression = arg.expr.Expand(definitions, referencedExpressions)
                });
            }
            return functionDef.Expression!.Expand(currentDefinitions, referencedExpressions);
        }
        private static IExpression Expand(VariableExpression expression, IEnumerable<IMacroDefinition> definitions, ISet<IExpression> referencedExpressions)
        {
            var def = definitions.FirstOrDefault(x => x.Name == expression.Value.Value && x.Args == null && x.Expression != null);
            if (def == null)
                return expression;
            return def.Expression!.Expand(definitions, referencedExpressions);
        }
        private static IExpression Expand(UnknownExpression expression, IEnumerable<IMacroDefinition> definitions, ISet<IExpression> referencedExpressions)
        {
            var expandedDefinitions = expression.Expressions
                .Select(x => x.Expand(definitions, referencedExpressions))
                .ToArray();

            var unknownExpression = new UnknownExpression()
            {
                Expressions = expandedDefinitions
            };

            var serializedExpression = unknownExpression.Serialize(null);

            return MacroParser.ParseExpression(serializedExpression);
        }

        private sealed class MacroExpansionGraphNode
        {
            public required IExpression Expression { get; init; }
            public List<MacroExpansionGraphNode> ReferencedExpressions { get; } = new();
            public bool HasSelfReferencingLoop()
            {
                return HasSelfReferencingLoop(this, new());
                static bool HasSelfReferencingLoop(MacroExpansionGraphNode node, List<IExpression> visited)
                {
                    if (visited.Contains(node.Expression))
                        return true;
                    visited.Add(node.Expression);
                    foreach (var item in node.ReferencedExpressions)
                    {
                        if (HasSelfReferencingLoop(item, visited))
                            return true;
                    }
                    visited.RemoveAt(visited.Count - 1);
                    return false;
                }
            }

        }
        #endregion

        #region IsConstLiteral
        /// <summary>
        /// Check if expression in a compile-time-const
        /// </summary>
        public static bool IsConstLiteral(this IExpression expression, IEnumerable<IMacroDefinition>? definitions = null)
        {
            definitions ??= Enumerable.Empty<IMacroDefinition>();
            expression = expression.Expand(definitions);
            return expression switch
            {
                GroupExpression e => IsConstLiteral(e.Expression, definitions),
                CallExpression e => false,
                CastExpression e => IsConstLiteral(e.Value, definitions),
                ConstantExpression e => true,
                KeywordExpression e => false,
                VariableExpression e => false,
                UnaryOperatorExpression e => IsConstLiteral(e.Expression, definitions),
                BinaryOperatorExpression e => IsConstLiteral(e.LeftExpression, definitions) && IsConstLiteral(e.RightExpression, definitions),
                TernaryOperatorExpression e => IsConstLiteral(e.Condition, definitions) && IsConstLiteral(e.TrueExpression, definitions) && IsConstLiteral(e.FalseExpression, definitions),
                UnknownExpression e => e.Expressions.All(x => IsConstLiteral(x, definitions)),
                _ => throw new NotSupportedException()
            };
        }
        #endregion
    }
}
