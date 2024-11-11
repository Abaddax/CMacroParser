namespace CMacroParser.Contracts
{
    public interface IDeducedType
    {
        string Type { get; }
        LiteralType Deduced { get; }

        internal static IDeducedType Create(string typeName, LiteralType deducedLiteralType) => new DeducedType() { Type = typeName, Deduced = deducedLiteralType };

        private class DeducedType : IDeducedType
        {
            public string Type { get; init; } = string.Empty;
            public LiteralType Deduced { get; init; } = LiteralType.unknown;


            public override string ToString()
            {
                return Deduced switch
                {
                    LiteralType.custom => Type,
                    _ => Deduced.ToString()
                };
            }
        }
    }
}
