using CMacroParser.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Tokens
{
    internal class LiteralToken : TokenBase
    {
        public override TokenType TokenType => TokenType.Literal;
        public string OriginalContent { get; init; }
        public LiteralType LiteralType { get; init; }
    }
}
