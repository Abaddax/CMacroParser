using CMacroParser.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Definitions
{
    internal class FunctionDefinition : IDefinition
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
