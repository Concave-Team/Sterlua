using Sterlua.CodeGen.Parser;
using Sterlua.CodeGen.Parser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic.Symbols
{
    public class FunctionSymbol : Symbol
    {
        public IdentifierType ReturnType;
        public List<Argument> Arguments;

        public FunctionSymbol(string name, IdentifierType returnType, List<Argument> arguments)
        {
            Name = name;
            ReturnType = returnType;
            Arguments = arguments;
        }
    }
}
