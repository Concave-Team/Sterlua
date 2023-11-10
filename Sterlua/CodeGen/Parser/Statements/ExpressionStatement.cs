using Sterlua.CodeGen.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Parser.Statements
{
    public class ExpressionStatement : Statement
    {
        public Expression expr;

        public ExpressionStatement(Expression expr)
        {
            this.expr = expr;
        }
    }
}
