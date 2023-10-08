using CMacroParser.Contracts;

namespace CMacroParser.Models.Tokens
{
    internal abstract class TokenBase : IToken
    {
        public abstract TokenType TokenType { get; }
        public string Value { get; init; }

        public override string ToString()
        {
            return $"{TokenType}: '{Value}'";
        }
    }
}
