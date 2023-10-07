using CMacroParser.Contracts;
using CMacroParser.Models.Definitions;
using CMacroParser.Models.Expressions;
using CMacroParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Parser
{
    public static class Parser
    {
        //https://en.cppreference.com/w/cpp/language/operator_precedence
        internal static readonly Dictionary<string, int> OperationPrecedence = new Dictionary<string, int>
        {
            { "*", 5 },
            { "/", 5 },
            { "%", 5 },
            { "+", 6 },
            { "-", 6 },
            { "<<", 7 },
            { ">>", 7 },
            { "<=>", 8 }, //?
            { "<", 9 },
            { "<=", 9 },
            { ">", 9 },
            { ">=", 9 },
            { "==", 10 },
            { "!=", 10 },
            { "&", 11 },
            { "^", 12 },
            { "|", 13 },
            { "&&", 14 },
            { "||", 15 },
            { "=", 16 },
            { "+=", 16 },
            { "-=", 16 },
            { "*=", 16 },
            { "/=", 16 },
            { "%=", 16 },
            { "<<=", 16 },
            { ">>=", 16 },
            { "&=", 16 },
            { "^=", 16 },
            { "|=", 16 },
            { ",", 17 },
        };
        //https://stackoverflow.com/questions/5563000/implicit-type-conversion-rules-in-c-operators
        internal static readonly Dictionary<LiteralType, int> TypePrecedence = new Dictionary<LiteralType, int>
        {
            { LiteralType.@string, 1 },
            { LiteralType.@char, 2 },

            { LiteralType.@decimal, 1 },
            { LiteralType.@double, 2 },
            { LiteralType.@float, 3 },
            { LiteralType.@ulong, 4 },
            { LiteralType.@long, 5 },
            { LiteralType.@uint, 6 },
            { LiteralType.@int, 7 },
            { LiteralType.@ushort, 8 },
            { LiteralType.@short, 9 },
            { LiteralType.@byte, 10 },

            { LiteralType.@bool, 1 }
        };

        public static IDefinition ParseDefinition(this string definition)
        {
            definition = definition.Trim();
            if (definition.StartsWith("#define "))
                definition = definition[8..];

            ReadOnlySpan<IToken> tokens = definition.Tokenize().ToArray();

            if (!tokens.IsCall())
            {
                var name = (VariableExpression)tokens.ParseVariable(out int skip);
                if (skip >= tokens.Length)
                {
                    return new VariableDefinition()
                    {
                        Name = name.Value.Value,
                        Expression = null
                    };
                }
                else
                {
                    var expresssion = ParseExpression(tokens[skip..]);
                    return new VariableDefinition()
                    {
                        Name = name.Value.Value,
                        Expression = expresssion
                    };
                }
            }
            else
            {
                var func = (CallExpression)tokens.ParseCall(out int skip);
                if (skip >= tokens.Length)
                {
                    return new FunctionDefinition()
                    {
                        Name = func.Value.Value,
                        Args = func.Arguments.Select(x => x.Serialize()).ToArray(),
                        Expression = null
                    };
                }
                else
                {
                    var expresssion = ParseExpression(tokens[skip..]);
                    return new FunctionDefinition()
                    {
                        Name = func.Value.Value,
                        Args = func.Arguments.Select(x => x.Serialize()).ToArray(),
                        Expression = expresssion
                    };
                }
            }
        }
        
        
        public static IExpression ParseExpression(this string expression)
        {
            var tokens = expression.Tokenize();
            return ParseExpression(tokens);
        }
        internal static IExpression ParseExpression(IEnumerable<IToken> expressionTokens)
        {
            return ParseExpression((ReadOnlySpan<IToken>)expressionTokens.ToArray());
        }
        internal static IExpression ParseExpression(ReadOnlySpan<IToken> expressionTokens)
        {
            int pos = 0;
            IExpression last = null;
            do
            {
                var tokens = expressionTokens[pos..];
                int skip;
                IExpression expression;
                if (tokens.IsCall())
                    expression = tokens.ParseCall(out skip);
                else if (tokens.IsOperator())
                    expression = tokens.ParseOperator(out skip);
                else if (tokens.IsCast())
                    expression = tokens.ParseCast(out skip);
                else if (tokens.IsConstant())
                    expression = tokens.ParseConstant(out skip);
                else if (tokens.IsVariable())
                    expression = tokens.ParseVariable(out skip);
                else
                    throw new NotSupportedException();
                pos += skip;

                //Append to last or set
                last = last?.Concat(expression) ?? expression;
            }
            while (pos < expressionTokens.Length);
            return last;

            if (pos != expressionTokens.Length)
                throw new Exception($"Unable to process entire expression. Multiple expressions are not supported. Unprocessed: {expressionTokens[pos..].Serialize()}");
            //return expression;
        }
    }
}   
