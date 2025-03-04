﻿using CMacroParser.Contracts;

namespace CMacroParser.Models.Definitions
{
    internal class FunctionDefinition : IMacroDefinition
    {
        public string Name { get; init; }
        public string[] Args { get; init; }
        public IExpression? Expression { get; init; }

        public string Serialize(ISerializerOptions? options)
        {
            return $"{Name}({string.Join(", ", Args)}) = {Expression?.Serialize(options) ?? "empty"}";
        }
        public override string ToString()
        {
            return Serialize(null);
        }
    }
}
