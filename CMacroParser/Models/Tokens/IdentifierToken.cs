using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal sealed class IdentifierToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Identifier;
        public required bool IsCall { get; init; }
    }
}
