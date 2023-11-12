using Sterlua.CodeGen.Semantic.BAST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic.BAST.Statements
{
    public class BExpressionStatement : BStatement
    {
        public BExpression expr;

        public BExpressionStatement(BExpression expr)
        {
            this.expr = expr;
        }
    }
}
