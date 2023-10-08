using CMacroParser.Contracts;

namespace CMacroParser.Models.Definitions
{
    internal class FunctionDefinition : IMacroDefinition
    {
        public string Name { get; init; }
        public string[] Args { get; init; }
        public IExpression? Expression { get; init; }
        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
