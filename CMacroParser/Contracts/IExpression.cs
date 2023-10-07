using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Contracts
{
    public interface IExpression
    {
        /// <summary>
        /// Token this expression contains of
        /// </summary>
        IEnumerable<IToken> Tokens { get; }
        /// <summary>
        /// Try to resolve unknown expressions with known maros from <paramref name="definitions"/>
        /// </summary>
        /// <returns>true if something got replaced</returns>
        bool Expand(IEnumerable<IDefinition> definitions);
        /// <summary>
        /// Serialize expression to text
        /// </summary>
        string Serialize();
    }
}
