using Sterlua.CodeGen.Semantic.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic.BAST.Expressions
{
    public class BCallFunctionExpression : BExpression
    {
        public FunctionSymbol symbol;
        public List<object> parameters;

        public BCallFunctionExpression(FunctionSymbol symbol, List<object> parameters)
        {
            this.symbol = symbol;
            this.parameters = parameters;
        }
    }
}
