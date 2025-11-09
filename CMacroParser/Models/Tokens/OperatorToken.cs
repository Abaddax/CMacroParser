using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal sealed class OperatorToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Operator;
    }
}
