using Sterlua.CodeGen.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic.Symbols
{
    public class VariableSymbol : Symbol
    {
        public IdentifierType Type;
        public object PrimaryValue;

        public VariableSymbol(string name, IdentifierType type, object primaryValue)
        {
            Name = name;
            Type = type;
            PrimaryValue = primaryValue;
        }
    }
}
