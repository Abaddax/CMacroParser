using Abaddax.CMacroParser.Contracts;

namespace Abaddax.CMacroParser.Models.Tokens
{
    internal sealed class PunctuatorToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Punctuator;
    }
}
