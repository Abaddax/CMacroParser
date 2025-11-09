using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal sealed class PunctuatorToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Punctuator;
    }
}
