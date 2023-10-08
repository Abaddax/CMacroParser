using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal class OperatorToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Operator;
    }
}
