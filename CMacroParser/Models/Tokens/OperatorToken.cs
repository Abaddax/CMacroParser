using Abaddax.CMacroParser.Contracts;

namespace Abaddax.CMacroParser.Models.Tokens
{
    internal sealed class OperatorToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Operator;
    }
}
