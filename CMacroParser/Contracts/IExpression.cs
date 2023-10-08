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
        /// Token this expression consists of
        /// </summary>
        IEnumerable<IToken> Tokens { get; }

        /// <summary>
        /// Check if any unkown expressions exit within this expression
        /// </summary>
        bool ContainsUnknown(IEnumerable<IMacroDefinition> definitions);

        /// <summary>
        /// Try to resolve unknown expressions with known macros from <paramref name="definitions"/>
        /// </summary>
        /// <returns>Expression with known expressions from <paramref name="definitions"/></returns>
        IExpression Expand(IEnumerable<IMacroDefinition> definitions);

        /// <summary>
        /// Serialize expression to text
        /// </summary>
        string Serialize();
    }
}
