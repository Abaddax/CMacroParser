using Abaddax.CMacroParser.Contracts;

namespace Abaddax.CMacroParser.Models.Tokens
{
    internal sealed class CommentToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Comment;
    }
}
