using CMacroParser.Contracts;

namespace CMacroParser.Models.Definitions
{
    internal sealed class FunctionDefinition : IMacroDefinition
    {
        public required string Name { get; init; }
        public required string[] Args { get; init; }
        public required IExpression? Expression { get; init; }

        public string Serialize(ISerializerOptions? options)
        {
            return $"{Name}({string.Join(", ", Args)}) = {Expression?.Serialize(options) ?? "empty"}";
        }
        public override string ToString()
        {
            return Serialize(null);
        }
    }
}
