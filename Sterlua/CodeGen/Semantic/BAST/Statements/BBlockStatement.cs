using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic.BAST.Statements
{
    public class BBlockStatement : BStatement
    {
        public List<BStatement> statements;

        public BBlockStatement(List<BStatement> statements)
        {
            this.statements = statements;
        }
    }
}
