using Abaddax.CMacroParser.Contracts;

namespace Abaddax.CMacroParser.Models.Definitions
{
    internal sealed class VariableDefinition : IMacroDefinition
    {
        public required string Name { get; init; }
        public string[]? Args => null;
        public required IExpression? Expression { get; init; }

        public string Serialize(ISerializerOptions? options)
        {
            return $"{Name} = {Expression?.Serialize(options) ?? "empty"}";
        }
        public override string ToString()
        {
            return Serialize(null);
        }
    }
}
