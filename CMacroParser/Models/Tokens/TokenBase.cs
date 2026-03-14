using Abaddax.CMacroParser.Contracts;

namespace Abaddax.CMacroParser.Models.Tokens
{
    internal abstract class TokenBase : IToken
    {
        public abstract TokenType TokenType { get; }
        public required string Value { get; init; }

        public override string ToString()
        {
            return $"{TokenType}: '{Value}'";
        }
    }
}
