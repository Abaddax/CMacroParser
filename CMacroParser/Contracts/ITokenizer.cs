using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Contracts
{
    internal interface ITokenizer
    {
        TokenType TokenType { get; }
        public string Value { get; }
        public int StartPosition { get; }
        public int Length { get; }
    }
}
