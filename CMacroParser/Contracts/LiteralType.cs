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
