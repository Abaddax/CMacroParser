namespace Abaddax.CMacroParser.Contracts
{
    public interface IToken
    {
        TokenType TokenType { get; }
        string Value { get; }
    }
}
