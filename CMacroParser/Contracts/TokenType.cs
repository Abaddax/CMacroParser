using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Contracts
{
    public enum TokenType
    {
        Punctuator,
        Keyword,
        Identifier,
        Literal,
        Operator,
        Comment
    }
}
