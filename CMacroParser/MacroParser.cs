using CMacroParser.Contracts;
using CMacroParser.Parser;

namespace CMacroParser
{
    public static class MacroParser
    {
        /// <summary>
        /// Parse macro definition
        /// </summary>
        /// <param name="definition">#define A 0</param>
        /// <returns></returns>
        public static IMacroDefinition ParseMacro(string definition)
        {
            return definition.ParseDefinition();
        }
        /// <summary>
        /// Parse macro expression
        /// </summary>
        /// <param name="expression">(int)FUNC(A, B)</param>
        /// <returns></returns>
        public static IExpression ParseExpression(string expression)
        {
            return expression.ParseExpression();
        }
    }
}
