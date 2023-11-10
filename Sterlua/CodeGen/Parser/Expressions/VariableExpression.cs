using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Parser.Expressions
{
    public class VariableExpression : Expression
    {
        public string Name;

        public VariableExpression(string name)
        {
            Name = name;
        }
    }
}
