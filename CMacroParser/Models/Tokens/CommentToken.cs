using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal class CommentToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Comment;
    }
}
