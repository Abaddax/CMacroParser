using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Contracts
{
    public interface ISerializerOptions
    {
        string GetLiteralSuffix(LiteralType literalType);

        public static ISerializerOptions Default { get; } = new SerializerOptions()
        {
            LiteralSuffix = new Dictionary<LiteralType, string>()
            {
                { LiteralType.@float, "f" },
                { LiteralType.@double, "" },
                { LiteralType.@decimal, "m" },
                { LiteralType.@int, "" },
                { LiteralType.@uint, "u" },
                { LiteralType.@long, "l" },
                { LiteralType.@ulong, "ul" },
                { LiteralType.@short, "" },
                { LiteralType.@ushort, "" }
            }
        };

        protected class SerializerOptions : ISerializerOptions
        {
            public Dictionary<LiteralType, string> LiteralSuffix { get; init; } = new();

            public string GetLiteralSuffix(LiteralType literalType)
            {
                if (LiteralSuffix?.TryGetValue(literalType, out var value) ?? false)
                    return value;
                return string.Empty;
            }
        }
    }
}
