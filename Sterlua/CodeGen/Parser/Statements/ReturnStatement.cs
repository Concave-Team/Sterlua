using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sterlua.CodeGen.Parser.Expressions;

namespace Sterlua.CodeGen.Parser.Statements
{
    public class ReturnStatement : Statement
    {
        public Expression exp;

        public ReturnStatement(Expression exp)
        {
            this.exp = exp;
        }
    }
}
