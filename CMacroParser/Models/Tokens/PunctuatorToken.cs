using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal class PunctuatorToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Punctuator;
    }
}
