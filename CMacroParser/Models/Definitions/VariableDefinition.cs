using CMacroParser.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Definitions
{
    internal class VariableDefinition : IDefinition
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
