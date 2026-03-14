using Abaddax.CMacroParser.Contracts;

namespace Abaddax.CMacroParser.Models.Tokens
{
    internal sealed class LiteralToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Literal;
        public required string OriginalContent { get; init; }
        public required LiteralType LiteralType { get; init; }
    }
}
