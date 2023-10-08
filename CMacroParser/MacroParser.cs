using CMacroParser.Contracts;
using CMacroParser.Parser;

namespace CMacroParser
{
    public static class MacroParser
    {
        public static IMacroDefinition ParseMacro(string definition)
        {
            return definition.ParseDefinition();
        }

        public static IExpression ParseExpression(string expression)
        {
            return expression.ParseExpression();
        }
    }
}
