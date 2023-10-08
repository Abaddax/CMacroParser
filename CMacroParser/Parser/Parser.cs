using CMacroParser.Contracts;
using CMacroParser.Models.Definitions;
using CMacroParser.Models.Expressions;
using CMacroParser.Tokenizer;

namespace CMacroParser.Parser
{
    public static class Parser
    {
        //https://en.cppreference.com/w/cpp/language/operator_precedence
        internal static readonly IReadOnlyDictionary<string, int> OperationPrecedence = new Dictionary<string, int>
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
        internal static readonly IReadOnlyDictionary<LiteralType, int> TypePrecedence = new Dictionary<LiteralType, int>
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

        public static IMacroDefinition ParseDefinition(this string definition)
        {
            definition = definition.Trim();
            if (definition.StartsWith("#define "))
                definition = definition[8..];

            ReadOnlySpan<IToken> tokens = definition.Tokenize().ToArray();

            if (!tokens.IsCall())
            {
                var name = (VariableExpression)tokens.ReadVariable(out int skip);
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
                var func = (CallExpression)tokens.ReadCall(out int skip);
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
                IExpression expression = ParseSingleExpression(tokens, out int skip);
                pos += skip;

                //Append to last or set
                last = last?.Concat(expression) ?? expression;
            }
            while (pos < expressionTokens.Length);
            return last;
        }
        internal static IExpression ParseSingleExpression(ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (tokens.IsGroup() && !tokens.IsCast())
            {
                tokens = tokens.ReadGroup(out skip);
                var expression = ParseExpression(tokens);
                if (expression is GroupExpression)
                    return expression;
                return new GroupExpression()
                {
                    Expression = ParseExpression(tokens)
                };
            }
            else if (tokens.IsCall())
                return tokens.ReadCall(out skip);
            else if (tokens.IsOperator())
                return tokens.ReadOperator(out skip);
            else if (tokens.IsCast())
                return tokens.ReadCast(out skip);
            else if (tokens.IsConstant())
                return tokens.ReadConstant(out skip);
            else if (tokens.IsVariable())
                return tokens.ReadVariable(out skip);
            else
                throw new NotSupportedException();
        }
    }
}
