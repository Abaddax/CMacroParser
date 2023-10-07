using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Contracts
{
    public interface IDefinition
    {
        /// <summary>
        /// Name of the macro
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Macro arguments
        /// </summary>
        string[]? Args { get; }
        /// <summary>
        /// Expression this macro
        /// </summary>
        IExpression? Expression { get; }
        /// <summary>
        /// Serialize definition to text
        /// </summary>
        string Serialize();
    }
}
