using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal class KeywordToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Keyword;
    }
}
