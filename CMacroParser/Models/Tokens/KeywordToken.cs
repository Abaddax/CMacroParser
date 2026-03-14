using Abaddax.CMacroParser.Contracts;

namespace Abaddax.CMacroParser.Models.Tokens
{
    internal sealed class KeywordToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Keyword;
    }
}
