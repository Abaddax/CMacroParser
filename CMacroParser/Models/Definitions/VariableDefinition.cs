using CMacroParser.Contracts;

namespace CMacroParser.Models.Definitions
{
    internal class VariableDefinition : IMacroDefinition
    {
        public string Name { get; init; }
        public string[]? Args => null;
        public IExpression? Expression { get; init; }
        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
