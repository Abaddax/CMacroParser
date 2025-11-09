using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal sealed class KeywordToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Keyword;
    }
}
