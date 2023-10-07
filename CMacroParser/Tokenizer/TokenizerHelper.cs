using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Tokenizer
{
    internal static class TokenizerHelper
    {
        public static bool IsSeperator(this ReadOnlySpan<char> chars)
        {
            if (chars.Length == 0)
                return false;
            return Tokenizer.Separators.Contains(chars[0]);
        }
        public static bool IsPunctuator(this ReadOnlySpan<char> chars)
        {
            if (chars.Length == 0)
                return false;
            return Tokenizer.Punctuators.Contains(chars[0]);
        }
        public static bool IsKeyword(this ReadOnlySpan<char> chars)
        {
            if (chars.Length == 0)
                return false;
            foreach (var keyword in Tokenizer.Keywords)
            {
                if (!chars.StartsWith(keyword))
                    continue;
                if (chars.Length == keyword.Length)
                    return true;
                //Check if not concatinated with something else e.g. int_max
                if (Tokenizer.Separators.Contains(chars[keyword.Length]) ||
                    Tokenizer.Punctuators.Contains(chars[keyword.Length]))
                    return true;
            }
            return false;
        }
        public static bool IsOperator(this ReadOnlySpan<char> chars)
        {
            if (chars.Length == 0)
                return false;
            return Tokenizer.Operators.Contains(chars[0]);
        }
        public static bool IsLiteral(this ReadOnlySpan<char> chars)
        {
            if (chars.Length == 0)
                return false;
            if (chars.StartsWith("true") ||
                chars.StartsWith("false")) //Technically an identifier but this is easier
                return true;
            return Tokenizer.Digits.Contains(chars[0]) ||
                char.ToLowerInvariant(chars[0]) == '\'' ||
                char.ToLowerInvariant(chars[0]) == '\"' ||
                char.ToLowerInvariant(chars[0]) == '.';
        }
        public static bool IsComment(this ReadOnlySpan<char> chars)
        {
            if (chars.Length == 0)
                return false;
            return chars.StartsWith("//") ||
                chars.StartsWith("/*") ||
                chars.StartsWith("*/");
        }
        public static bool IsIdentifier(this ReadOnlySpan<char> chars)
        {
            if (chars.Length == 0)
                return false;
            bool isOther = IsSeperator(chars) ||
                IsPunctuator(chars) ||
                IsKeyword(chars) ||
                IsOperator(chars) ||
                IsLiteral(chars) ||
                IsComment(chars);
            return !isOther;
        }


        public static void SkipSeperator(this ReadOnlySpan<char> chars, out int skip)
        {
            skip = 0;
            while (IsSeperator(chars[skip..]))
                skip++;
            return;
        }
        public static IToken ReadPunctuator(this ReadOnlySpan<char> chars, out int skip)
        {
            if (!IsPunctuator(chars))
                throw new InvalidOperationException();
            skip = 1;
            return new PunctuatorToken() { Value = chars[0].ToString() };
        }
        public static IToken ReadKeyword(this ReadOnlySpan<char> chars, out int skip)
        {
            if (!IsKeyword(chars))
                throw new InvalidOperationException();
            List<string> keywords = new List<string>();
            foreach (var keyword in Tokenizer.Keywords)
            {
                if (chars.StartsWith(keyword))
                    keywords.Add(keyword); //Multiple possible e.g. do and double
            }
            var value = keywords.MaxBy(k => k.Length)!;
            skip = value.Length;
            return new KeywordToken()
            {
                Value = value
            };
        }
        public static IToken ReadOperator(this ReadOnlySpan<char> chars, out int skip)
        {
            if (!IsOperator(chars))
                throw new InvalidOperationException();
            skip = 0;
            while (IsOperator(chars[skip..]))
                skip++;
            return new OperatorToken()
            {
                Value = chars[0..skip].ToString()
            };
        }
        public static IToken ReadLiteral(this ReadOnlySpan<char> chars, out int skip)
        {
            if (!IsLiteral(chars))
                throw new InvalidOperationException();
            //Bool
            if (chars.StartsWith("true"))
            {
                skip = 4;
                return new LiteralToken()
                {
                    Value = "true",
                    LiteralType = LiteralType.@bool,
                    OriginalContent = chars[0..skip].ToString()
                };
            }
            else if (chars.StartsWith("false"))
            {
                skip = 5;
                return new LiteralToken()
                {
                    Value = "false",
                    LiteralType = LiteralType.@bool,
                    OriginalContent = chars[0..skip].ToString()
                };
            }
            //Char
            else if (chars.StartsWith("\'"))
            {
                skip = 0;
                while (true)
                {
                    if (++skip > chars.Length)
                        throw new Exception("Input ended before char-literal got closed. ({chars})");
                    if (chars[skip] == '\\')
                        skip++;
                    else if (chars[skip] == '\'')
                        break;
                }
                return new LiteralToken()
                {
                    Value = chars[0..++skip].ToString(),
                    LiteralType = LiteralType.@char,
                    OriginalContent = chars[0..skip].ToString()
                };
            }
            //String
            else if (chars.StartsWith("\""))
            {
                skip = 0;
                while (true)
                {
                    if (++skip > chars.Length)
                        throw new Exception($"Input ended before string-literal got closed. ({chars})");
                    if (chars[skip] == '\\')
                        skip++;
                    else if (chars[skip] == '\"')
                        break;
                }
                return new LiteralToken()
                {
                    Value = chars[0..++skip].ToString(),
                    LiteralType = LiteralType.@string,
                    OriginalContent = chars[0..skip].ToString()
                };
            }
            //number
            else if (chars[0] == '.' || Tokenizer.Digits.Contains(chars[0]))
            {
                skip = 0;

                int numBase = 10;

                //Check for integer base
                if (chars[0] == '0')
                {
                    if(chars.Length==1)
                    {
                        skip = 0;
                    }
                    //Hex
                    else if (char.ToLowerInvariant(chars[1]) == 'x')
                    {
                        numBase = 16;
                        skip = 2;
                    }
                    //Binary
                    else if (char.ToLowerInvariant(chars[1]) == 'b')
                    {
                        numBase = 2;
                        skip = 2;
                    }
                    //Octal
                    else
                    {
                        numBase = 8;
                        skip = 1;
                    }
                }

                while (skip < chars.Length)
                {
                    if (numBase <= 10 && Tokenizer.Digits.Contains(chars[skip]))
                        skip++;
                    else if (numBase == 16 && (Tokenizer.Digits.Contains(chars[skip]) || Tokenizer.HexDigits.Contains(char.ToLowerInvariant(chars[skip]))))
                        skip++;
                    else if (chars[skip] == '.')
                    {
                        numBase = 10;
                        skip++;
                    }
                    else if (numBase==10 && char.ToLowerInvariant(chars[skip]) == 'e')
                    {
                        skip++;
                        if (chars[skip] == '+')
                            skip++;
                        if (chars[skip] == '-')
                            skip++;
                    }
                    else if (Tokenizer.NumberEnd.Contains(char.ToLowerInvariant(chars[skip])))
                        skip++;
                    else
                        break;
                }

                return ParseNumericLiteral(chars[0..skip]);
            }
            else
                throw new Exception("unreachable");
        }
        public static IToken ReadIdentifier(this ReadOnlySpan<char> chars, out int skip)
        {
            if (!IsIdentifier(chars))
                throw new InvalidOperationException();
            skip = 0;
            //Literal and keyword are needed incase of concatination (e.g. Func2int)
            while (IsIdentifier(chars[skip..]) || IsLiteral(chars[skip..]) || IsKeyword(chars[skip..]))
                skip++;
            bool isCall = false;
            if (chars.Length > skip && chars[skip] == '(')
                isCall = true;
            return new IdentifierToken()
            {
                Value = chars[0..skip].ToString(),
                IsCall = isCall
            };
        }


        private static IToken ParseNumericLiteral(this ReadOnlySpan<char> literal)
        {
            var value = literal.ToString().ToLowerInvariant();
            //float/double/decimal
            if (value.Contains('.') || (value.Contains('e') && !value.Contains('x')))
            {
                if (value.StartsWith('.'))
                    value = '0' + value;
                var type = LiteralType.@double;
                if (value.Contains('f'))
                    type = LiteralType.@float;
                else if (value.Contains('d'))
                    type = LiteralType.@double;
                else if (value.Contains('l'))
                    type = LiteralType.@decimal;
                value = value.Replace("f", "").Replace("d", "").Replace("l", "");

                return (type, value) switch
                {
                    var (t, v) when t == LiteralType.@float && TryParse(() => float.Parse(v, NumberStyles.Float, CultureInfo.InvariantCulture), out var _value) =>
                        new LiteralToken() { Value = _value.ToString(CultureInfo.InvariantCulture), LiteralType = LiteralType.@float, OriginalContent = literal.ToString() },
                    var (t, v) when t == LiteralType.@double && TryParse(() => double.Parse(v, NumberStyles.Float, CultureInfo.InvariantCulture), out var _value) =>
                        new LiteralToken() { Value = _value.ToString(CultureInfo.InvariantCulture), LiteralType = LiteralType.@double, OriginalContent = literal.ToString() },
                    var (t, v) when t == LiteralType.@decimal && TryParse(() => decimal.Parse(v, NumberStyles.Float, CultureInfo.InvariantCulture), out var _value) =>
                        new LiteralToken() { Value = _value.ToString(CultureInfo.InvariantCulture), LiteralType = LiteralType.@decimal, OriginalContent = literal.ToString() },
                    _ => throw new Exception($"Unable to parse floating-point-literal. ({literal})")
                };
            }
            //integer
            else
            {
                int numBase = 10;
                if (value[0] == '0')
                {
                    if (value.Length == 1)
                    {
                        value = "0";
                    }
                    //Hex
                    else if (value[1] == 'x')
                    {
                        numBase = 16;
                        value = string.Concat(value.Skip(2));
                    }
                    //Binary
                    else if (value[1] == 'b')
                    {
                        numBase = 2;
                        value = string.Concat(value.Skip(2));
                    }
                    //Octal
                    else
                    {
                        numBase = 8;
                        value = string.Concat(value.Skip(1));
                    }
                }

                var type = LiteralType.@int;
                if (value.Contains('u') && value.Contains('l'))
                    type = LiteralType.@ulong;
                else if (value.Contains('l'))
                    type = LiteralType.@long;
                else if (value.Contains('u'))
                    type = LiteralType.@uint;
                value = value.Replace("u", "").Replace("l", "");

                return (type, value) switch
                {
                    var (t, v) when t == LiteralType.@int && TryParse(() => Convert.ToInt32(v, numBase), out var _value) =>
                        new LiteralToken() { Value = _value.ToString(), LiteralType = LiteralType.@int, OriginalContent = literal.ToString() },
                    var (t, v) when t == LiteralType.@uint && TryParse(() => Convert.ToUInt32(v, numBase), out var _value) =>
                        new LiteralToken() { Value = _value.ToString(), LiteralType = LiteralType.@uint, OriginalContent = literal.ToString() },
                    var (t, v) when t == LiteralType.@long && TryParse(() => Convert.ToInt64(v, numBase), out var _value) =>
                        new LiteralToken() { Value = _value.ToString(), LiteralType = LiteralType.@long, OriginalContent = literal.ToString() },
                    var (t, v) when t == LiteralType.@ulong && TryParse(() => Convert.ToUInt64(v, numBase), out var _value) =>
                        new LiteralToken() { Value = _value.ToString(), LiteralType = LiteralType.@ulong, OriginalContent = literal.ToString() },
                    _ => throw new Exception($"Unable to parse integer-literal. ({literal})")
                };
            }
        }
        private static bool TryParse<T>(Func<T> func, out T? value)
        {
            try
            {
                value = func.Invoke();
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }
    }
}
