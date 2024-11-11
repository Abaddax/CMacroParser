using CMacroParser.Contracts;
using CMacroParser.Models.Expressions;
using CMacroParser.Models.Tokens;
using static CMacroParser.Parser.Parser;

namespace CMacroParser.Parser
{
    internal static class ParserHelper
    {
        public static bool IsSequenceOf(this ReadOnlySpan<IToken> tokens, params Func<IToken, bool>[] sequence)
        {
            if (sequence.Length > tokens.Length)
                return false;
            for (int i = 0; i < sequence.Length; i++)
            {
                if (!sequence[i].Invoke(tokens[i]))
                    return false;
            }
            return true;
        }

        public static bool IsCall(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length < 3)
                return false;
            return tokens[0].IsCall();
        }
        public static bool IsCast(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length < 4)
                return false;
            if (tokens.IsSequenceOf(x => x.IsPunctuator("("), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsPunctuator(")"), x => !x.IsOperator()))
                return true; //(float)
            if (tokens.IsSequenceOf(x => x.IsPunctuator("("), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsPunctuator(")"), x => !x.IsOperator()))
                return true; //(long double)
            if (tokens.IsSequenceOf(x => x.IsPunctuator("("), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsPunctuator(")"), x => !x.IsOperator()))
                return true; //(long long int)
            if (tokens.IsSequenceOf(x => x.IsPunctuator("("), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsPunctuator(")"), x => !x.IsOperator()))
                return true; //(unsigned long long int)
            return false;
        }
        public static bool IsConstant(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length == 0)
                return false;
            return tokens[0].IsLiternal();
        }
        public static bool IsOperator(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length < 2)
                return false;
            if (tokens[0].IsOperator())
                return true;
            if (tokens.IsSequenceOf(x => x.IsIdentifier() || x.IsLiternal(), x => x.IsOperator("++") || x.IsOperator("--")))
                return true;  //a++
            return false;
        }
        public static bool IsVariable(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length == 0)
                return false;
            return !tokens[0].IsCall() && tokens[0].IsKeyword() || tokens[0].IsIdentifier();
        }


        public static IExpression ReadCall(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!tokens.IsCall())
                throw new InvalidOperationException();
            skip = 1;

            var argTokens = tokens[1..].ReadGroup(out var _skip);
            skip += _skip;
            var args = argTokens.ReadArgs();

            List<IExpression> argExpressions = new();
            foreach (ReadOnlySpan<IToken> arg in args)
            {
                var expr = Parser.ParseExpression(arg);
                argExpressions.Add(expr);
            }

            return new CallExpression()
            {
                Value = new IdentifierToken()
                {
                    Value = tokens[0].Value,
                    IsCall = true
                },
                Arguments = argExpressions.ToArray()
            };
        }
        public static IExpression ReadCast(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!tokens.IsCast())
                throw new InvalidOperationException();

            var targets = tokens.ReadGroup(out var _skip);

            skip = _skip;

            var valueExpr = Parser.ParseSingleExpression(tokens[skip..], out _skip);
            skip += _skip;

            return new CastExpression()
            {
                Value = valueExpr,
                TargetType = new KeywordToken()
                {
                    Value = string.Join(" ", targets.ToArray().Select(x => x.Value))
                }
            };
        }
        public static IExpression ReadConstant(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!tokens.IsConstant())
                throw new InvalidOperationException();
            skip = 1;
            return new ConstantExpression()
            {
                Value = (LiteralToken)tokens[0]
            };
        }
        public static IExpression ReadOperator(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!tokens.IsOperator())
                throw new InvalidOperationException();

            //++a
            if (tokens[0].IsOperator())
            {
                var @operator = (OperatorToken)tokens[0];
                skip = 1;

                var expression = Parser.ParseSingleExpression(tokens[1..], out int _skip);
                skip += _skip;
                return new UnaryOperatorExpression()
                {
                    Operator = @operator,
                    Expression = expression,
                    IsSuffixOperator = false,
                };
            }
            //a++
            else
            {
                var exprTokens = ReadGroupOrToken(tokens, out skip);
                var expression = Parser.ParseSingleExpression(exprTokens, out _);
                var @operator = (OperatorToken)tokens[skip++];
                return new UnaryOperatorExpression()
                {
                    Operator = @operator,
                    Expression = expression,
                    IsSuffixOperator = true
                };
            }
        }
        public static IExpression ReadVariable(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!IsVariable(tokens))
                throw new InvalidOperationException();

            skip = 1;
            return new VariableExpression()
            {
                Value = new IdentifierToken()
                {
                    Value = tokens[0].Value
                }
            };
        }


        public static IExpression Concat(this IExpression last, IExpression append)
        {
            if (last == null)
                return append;
            else if (append is UnaryOperatorExpression appendUnary && !appendUnary.IsSuffixOperator) //last +append
            {
                //Check operator order 
                //(1+2)*3 -> 1+(2*3)
                if (last is BinaryOperatorExpression lastBinary) //last=1+2 -> +append
                {
                    if (lastBinary.Operator.Value == "?" && appendUnary.Operator.Value == ":")
                    {
                        return new TernaryOperatorExpression()
                        {
                            Condition = lastBinary.LeftExpression,
                            Operator1 = lastBinary.Operator,
                            TrueExpression = lastBinary.RightExpression,
                            Operator2 = appendUnary.Operator,
                            FalseExpression = appendUnary.Expression
                        };
                    }

                    OperationPrecedence.TryGetValue(lastBinary.Operator.Value, out var precedence1);
                    OperationPrecedence.TryGetValue(appendUnary.Operator.Value, out var precedence2);

                    if (precedence1 > precedence2)
                    {
                        //Swaped
                        var rightExpr = new BinaryOperatorExpression()
                        {
                            LeftExpression = lastBinary.RightExpression,
                            Operator = appendUnary.Operator,
                            RightExpression = appendUnary.Expression,
                        };
                        return new BinaryOperatorExpression()
                        {
                            LeftExpression = lastBinary.LeftExpression,
                            Operator = lastBinary.Operator,
                            RightExpression = rightExpr,
                        };
                    }
                }

                //Normal
                return new BinaryOperatorExpression()
                {
                    LeftExpression = last,
                    Operator = appendUnary.Operator,
                    RightExpression = appendUnary.Expression,
                };
            }
            throw new Exception($"Unable to concat '{append}' to '{last}'");
        }


        public static bool IsGroup(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length < 3)
                return false;
            if (tokens[0].IsPunctuator("("))
                return true;
            return false;
        }
        public static ReadOnlySpan<IToken> ReadGroup(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            HashSet<char> openSeperator = new HashSet<char>("([{");
            HashSet<char> closeSeperator = new HashSet<char>(")]}");

            if (!tokens[0].IsPunctuator("(") &&
                !tokens[0].IsPunctuator("[") &&
                !tokens[0].IsPunctuator("{"))
                throw new InvalidOperationException("Tokens must start with '(', '{' or '['");
            int nesting = 1;
            skip = 1;
            while (nesting > 0)
            {
                if (tokens[skip].IsPunctuator(")") ||
                    tokens[skip].IsPunctuator("]") ||
                    tokens[skip].IsPunctuator("}"))
                    nesting--;
                else if (tokens[skip].IsPunctuator("(") ||
                    tokens[skip].IsPunctuator("[") ||
                    tokens[skip].IsPunctuator("{"))
                    nesting++;
                skip++;
            }
            return tokens[1..(skip - 1)];
        }
        public static ReadOnlySpan<IToken> ReadGroupOrToken(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (tokens.IsGroup())
                return tokens.ReadGroup(out skip);
            skip = 1;
            return tokens[0..1];
        }



        private static List<IToken[]> ReadArgs(this ReadOnlySpan<IToken> tokens)
        {
            List<IToken[]> ret = new();
            int lastIndex = 0;
            int index = 0;
            foreach (var token in tokens)
            {
                index++;
                if (token.IsPunctuator(","))
                {
                    var arg = tokens[lastIndex..(index - 1)];
                    lastIndex = index;
                    ret.Add(arg.ToArray());
                }
            }
            ret.Add(tokens[lastIndex..].ToArray());

            return ret;
        }

    }
}
