using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Contracts
{
    public interface IToken
    {
        TokenType TokenType { get; }
        string Value { get; }
    }
}
