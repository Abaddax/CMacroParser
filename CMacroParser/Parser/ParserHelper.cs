using CMacroParser.Contracts;
using CMacroParser.Models.Expressions;
using CMacroParser.Models.Tokens;
using CMacroParser.Tokenizer;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
            return tokens.IsSequenceOf(x => x.IsPunctuator("("), x => x.IsKeyword() || x.IsIdentifier(), x => x.IsPunctuator(")"));
        }
        public static bool IsConstant(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length == 0)
                return false;
            if(tokens.IsGroup()) //(1)
            {
                var innerTokens = ReadGroup(tokens, out _);
                return innerTokens.IsConstant();
            }
            return tokens[0].IsLiternal();
        }
        public static bool IsOperator(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length < 2)
                return false;
            //Unary
            if (tokens[0].IsOperator())
            {
                if (tokens[1].IsIdentifier() || tokens[1].IsLiternal())
                    return true; //++a
                if (tokens[1..].IsGroup())
                    return true; //!(a>b)
            }
            //Binary or ternary
            if (tokens.IsSequenceOf(x => x.IsIdentifier() || x.IsLiternal(), x => x.IsOperator()))
                return true;  //a++,a+b,a?b:c
            //Binary or ternary
            if (tokens.IsGroup())
            {
                ReadGroup(tokens, out var skip);
                if (tokens.Length <= skip)
                    return false;
                if (tokens[skip].IsOperator())
                    return true; //(a)+b,(a)?b:c
            }
            return false;
        }
        public static bool IsVariable(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length == 0)
                return false;
            if (tokens.IsGroup()) //(A)
            {
                var innerTokens = ReadGroup(tokens, out _);
                return innerTokens.IsVariable();
            }
            return !tokens[0].IsCall() && tokens[0].IsKeyword() || tokens[0].IsIdentifier();
        }

        public static IExpression ParseCall(this ReadOnlySpan<IToken> tokens, out int skip)
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
        public static IExpression ParseCast(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!tokens.IsCast())
                throw new InvalidOperationException();

            var target = tokens[1];
            skip = 3;

            var value = tokens[3..].ReadGroupOrToken(out int _skip);
            skip += _skip;

            var valueExpr = Parser.ParseExpression(value);
            return new CastExpression()
            {
                Value = valueExpr,
                TargetType = target,
            };
        }
        public static IExpression ParseConstant(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!tokens.IsConstant())
                throw new InvalidOperationException();
            skip = 0;
            while (tokens.IsGroup())
            {
                tokens = tokens.ReadGroup(out var _skip);
                skip += _skip;
            }
            skip += 1;
            return new ConstantExpression()
            {
                Value = (LiteralToken)tokens[0]
            };
        }
        public static IExpression ParseOperator(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!tokens.IsOperator())
                throw new InvalidOperationException();

            ReadOnlySpan<IToken> expr1, expr2, expr3;
            IToken @operator1, @operator2;

            //++a
            if (tokens[0].IsOperator())
            {
                @operator1 = tokens[0];
                expr1 = tokens[1..].ReadGroupOrToken(out skip);
                skip += 1;

                //++a
                return new UnaryOperatorExpression()
                {
                    Operator = (OperatorToken)@operator1,
                    Expression = Parser.ParseExpression(expr1),
                    IsSuffixOperator = false
                };
            }

            //a++, a+b, a+b+c, a?b:c 
            expr1 = tokens[0..].ReadGroupOrToken(out skip);
            operator1 = tokens[skip++];

            //a++
            if (!tokens[skip..].IsSequenceOf(x => x.IsIdentifier() || x.IsLiternal() || x.IsPunctuator("(")))
            {
                return new UnaryOperatorExpression()
                {
                    Operator = (OperatorToken)@operator1,
                    Expression = Parser.ParseExpression(expr1),
                    IsSuffixOperator = true
                };
            }

            expr2 = tokens[skip..].ReadGroupOrToken(out var _skip);
            skip += _skip;

            //a+b
            if (operator1.Value != "?")
            {
                return new BinaryOperatorExpression()
                {
                    LeftExpression = Parser.ParseExpression(expr1),
                    Operator = (OperatorToken)operator1,
                    RightExpression = Parser.ParseExpression(expr2)
                };
            }

            //a?b:c
            operator2 = tokens[skip++];
            expr3 = tokens[skip..].ReadGroupOrToken(out _skip);
            skip += _skip;

            return new TernaryOperatorExpression()
            {
                Condition = Parser.ParseExpression(expr1),
                Operator1 = (OperatorToken)operator1,
                TrueExpression = Parser.ParseExpression(expr2),
                Operator2 = (OperatorToken)operator2,
                FalseExpression = Parser.ParseExpression(expr3)
            };
        }
        public static IExpression ParseVariable(this ReadOnlySpan<IToken> tokens, out int skip)
        {
            if (!IsVariable(tokens))
                throw new InvalidOperationException();

            skip = 0;
            while (tokens.IsGroup())
            {
                tokens = tokens.ReadGroup(out var _skip);
                skip += _skip;
            }
            skip += 1;
            return new VariableExpression()
            {
                Value = new IdentifierToken()
                {
                    Value = tokens[0].Value
                }
            };
        }


        public static IExpression Concat(this IExpression last, IExpression expression)
        {
            if (last == null)
                return expression;
            if (expression is UnaryOperatorExpression append && !append.IsSuffixOperator)
            {
                if (last is BinaryOperatorExpression lastExpr)
                {
                    Parser.OperationPrecedence.TryGetValue(lastExpr.Operator.Value, out var precedence1);
                    Parser.OperationPrecedence.TryGetValue(append.Operator.Value, out var precedence2);

                    if (precedence1 > precedence2)
                    {
                        //Swaped
                        var rightExpr = new BinaryOperatorExpression()
                        {
                            LeftExpression = lastExpr.RightExpression,
                            Operator = append.Operator,
                            RightExpression = append.Expression,
                        };
                        return new BinaryOperatorExpression()
                        {
                            LeftExpression = lastExpr.LeftExpression,
                            Operator = lastExpr.Operator,
                            RightExpression = rightExpr,
                        };
                    }
                }
                //Normal
                return new BinaryOperatorExpression()
                {
                    LeftExpression = last,
                    Operator = append.Operator,
                    RightExpression = append.Expression,
                };
            }

            throw new Exception($"Unable to concat '{expression}' to '{last}'");
        }


        private static bool IsGroup(this ReadOnlySpan<IToken> tokens)
        {
            if (tokens.Length < 3)
                return false;
            if (tokens[0].IsPunctuator("("))
                return true;
            return false;
        }
        private static ReadOnlySpan<IToken> ReadGroup(this ReadOnlySpan<IToken> tokens, out int skip)
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
        private static ReadOnlySpan<IToken> ReadGroupOrToken(this ReadOnlySpan<IToken> tokens, out int skip)
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
