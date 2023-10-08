namespace CMacroParser.Contracts
{
    public interface IToken
    {
        TokenType TokenType { get; }
        string Value { get; }
    }
}
