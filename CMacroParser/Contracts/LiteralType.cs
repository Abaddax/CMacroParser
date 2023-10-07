using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Contracts
{
    public enum LiteralType
    {
        unknown,

        @void,

        @int,
        @uint,
        @long,
        @ulong,
        @short,
        @ushort,

        @byte,

        @double,
        @float,
        @decimal,

        @bool,

        @char,
        @string
    }
}
