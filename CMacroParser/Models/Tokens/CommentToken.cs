using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal sealed class CommentToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Comment;
    }
}
