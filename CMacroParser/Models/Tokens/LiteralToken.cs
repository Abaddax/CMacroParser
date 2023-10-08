using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal class LiteralToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Literal;
        public string OriginalContent { get; init; }
        public LiteralType LiteralType { get; init; }
    }
}
