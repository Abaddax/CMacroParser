using CMacroParser.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Tokens
{
    internal class OperatorToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Operator;
    }
}
