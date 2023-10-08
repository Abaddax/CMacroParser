using CMacroParser.Contracts;
using CMacroParser.Models.Tokens;

namespace CMacroParser.Parser
{
    internal static class TokenExtensions
    {
        public static bool IsIdentifier(this IToken token) =>
            token.TokenType == TokenType.Identifier;
        public static bool IsIdentifier(this IToken token, string identifier) =>
            token.TokenType == TokenType.Identifier && (token as IdentifierToken)?.Value == identifier;
        public static bool IsCall(this IToken token) =>
            token.TokenType == TokenType.Identifier && (token as IdentifierToken)?.IsCall == true;

        public static bool IsOperator(this IToken token) =>
            token.TokenType == TokenType.Operator;
        public static bool IsOperator(this IToken token, string @operator) =>
            token.TokenType == TokenType.Operator && (token as OperatorToken)?.Value == @operator;

        public static bool IsLiternal(this IToken token) =>
            token.TokenType == TokenType.Literal;
        public static bool IsLiternal(this IToken token, LiteralType type) =>
           token.TokenType == TokenType.Literal && (token as LiteralToken)?.LiteralType == type;

        public static bool IsKeyword(this IToken token) =>
            token.TokenType == TokenType.Keyword;
        public static bool IsKeyword(this IToken token, string keyword) =>
            token.TokenType == TokenType.Keyword && (token as KeywordToken)?.Value == keyword;

        public static bool IsPunctuator(this IToken token) =>
            token.TokenType == TokenType.Punctuator;
        public static bool IsPunctuator(this IToken token, string punctuator) =>
           token.TokenType == TokenType.Punctuator && (token as PunctuatorToken)?.Value == punctuator;
    }
}
