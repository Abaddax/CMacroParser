using CMacroParser.Contracts;
using CMacroParser.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Parser
{
    public static class ExpressionHelper
    {
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
            Parser.OperationPrecedence.TryGetValue(expression.Operator.Value, out var precedence);
            if (precedence == 9 || precedence == 10) //Binary operators
                return LiteralType.@bool;

            var leftType = DeduceType(expression.LeftExpression);
            var rightType = DeduceType(expression.RightExpression);
            Parser.TypePrecedence.TryGetValue(leftType, out var leftPrecedence);
            Parser.TypePrecedence.TryGetValue(rightType, out var rightPrecedence);

            if (leftPrecedence < rightPrecedence)
                return leftType;
            else if (leftPrecedence == rightPrecedence && leftType != rightType)
                throw new Exception($"Unable to deduce type of {leftType} and {rightType}.");
            else
                return rightType;

            int x = 0;
        }
        private static LiteralType DeduceType(TernaryOperatorExpression expression)
        {
            var trueType = DeduceType(expression.TrueExpression);
            var falseType = DeduceType(expression.FalseExpression);
            Parser.TypePrecedence.TryGetValue(trueType, out var truePrecedence);
            Parser.TypePrecedence.TryGetValue(falseType, out var falsePrecedence);

            if (falsePrecedence < truePrecedence)
                return falseType;
            else
                return trueType;
        }
    }
}
