using Sterlua.CodeGen.Semantic.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic.BAST.Statements
{
    public class BFunctionStatement : BStatement
    {
        public FunctionSymbol symbol;
        public BBlockStatement block;

        public BFunctionStatement(FunctionSymbol symbol, BBlockStatement block)
        {
            this.symbol = symbol;
            this.block = block;
        }
    }
}
