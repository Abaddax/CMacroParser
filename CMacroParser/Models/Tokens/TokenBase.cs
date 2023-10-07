using CMacroParser.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Models.Tokens
{
    internal abstract class TokenBase : IToken
    {
        public abstract TokenType TokenType { get; }
        public string Value { get; init; }

        public override string ToString()
        {
            return $"{TokenType}: '{Value}'";
        }
    }
}
