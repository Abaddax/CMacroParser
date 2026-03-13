using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Parser
{
    internal static class TokenExtensions
    {
        public static bool IsIdentifier(this IToken token) =>
            token.TokenType == TokenType.Identifier;
        public static bool IsIdentifier(this IToken token, string identifier) =>
            token.IsIdentifier() && (token as IdentifierToken)?.Value == identifier;
        public static bool IsIdentifier(this IToken token, params ReadOnlySpan<string> identifiers)
        {
            foreach(var identifier in identifiers)
            {
                if (token.IsIdentifier(identifier))
                    return true;
            }
            return false;
        }

        public static bool IsCall(this IToken token) =>
            token.TokenType == TokenType.Identifier && (token as IdentifierToken)?.IsCall == true;

        public static bool IsOperator(this IToken token) =>
            token.TokenType == TokenType.Operator;
        public static bool IsOperator(this IToken token, string @operator) =>
            token.IsOperator() && (token as OperatorToken)?.Value == @operator;
        public static bool IsOperator(this IToken token, params ReadOnlySpan<string> @operators)
        {
            foreach (var @operator in @operators)
            {
                if (token.IsOperator(@operator))
                    return true;
            }
            return false;
        }

        public static bool IsLiteral(this IToken token) =>
            token.TokenType == TokenType.Literal;
        public static bool IsLiteral(this IToken token, LiteralType type) =>
           token.IsLiteral() && (token as LiteralToken)?.LiteralType == type;
        public static bool IsLiteral(this IToken token, params ReadOnlySpan<LiteralType> types)
        {
            foreach(var type in types)
            {
                if(token.IsLiteral(type))
                    return true;
            }
            return false;
        }

        public static bool IsKeyword(this IToken token) =>
            token.TokenType == TokenType.Keyword;
        public static bool IsKeyword(this IToken token, string keyword) =>
            token.IsKeyword() && (token as KeywordToken)?.Value == keyword;
        public static bool IsKeyword(this IToken token, params ReadOnlySpan<string> keywords)
        {
            foreach(var keyword in keywords)
            {
                if(token.IsKeyword(keyword))
                    return true;
            }
            return false;
        }

        public static bool IsPunctuator(this IToken token) =>
            token.TokenType == TokenType.Punctuator;
        public static bool IsPunctuator(this IToken token, string punctuator) =>
           token.IsPunctuator() && (token as PunctuatorToken)?.Value == punctuator;
        public static bool IsPunctuator(this IToken token, params ReadOnlySpan<string> punctuators)
        {
            foreach(var punctuator in punctuators)
            {
                if (token.IsPunctuator(punctuator))
                    return true;
            }
            return false;
        }
    }
}
