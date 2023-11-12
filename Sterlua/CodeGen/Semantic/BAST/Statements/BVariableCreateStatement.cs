using Sterlua.CodeGen.Semantic.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic.BAST.Statements
{
    public class BVariableCreateStatement : BStatement
    {
        public VariableSymbol variable;

        public BVariableCreateStatement(VariableSymbol variable)
        {
            this.variable = variable;
        }
    }
}
