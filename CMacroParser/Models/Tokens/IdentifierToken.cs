using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal class IdentifierToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Identifier;
        public bool IsCall { get; init; }
    }
}
