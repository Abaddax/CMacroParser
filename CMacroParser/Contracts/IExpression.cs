namespace CMacroParser.Contracts
{
    public interface IExpression
    {
        /// <summary>
        /// Token this expression consists of
        /// </summary>
        IEnumerable<IToken> Tokens { get; }

        /// <summary>
        /// Serialize expression to text
        /// </summary>
        string Serialize(ISerializerOptions? options = null);
    }
}
