using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Parser.Statements
{
    public class Argument
    {
        public IdentifierType ArgType;
        public string ArgName;

        public Argument(IdentifierType argType, string argName)
        {
            ArgType = argType;
            ArgName = argName;
        }
    }

    public class FunctionStatement : Statement
    {
        public string Name;
        public BlockStatement Body;
        public IdentifierType ReturnType;
        public List<Argument> Arguments;

        public FunctionStatement(string name, BlockStatement body, IdentifierType returnType, List<Argument> arguments)
        {
            Name = name;
            Body = body;
            ReturnType = returnType;
            Arguments = arguments;
        }
    }
}
