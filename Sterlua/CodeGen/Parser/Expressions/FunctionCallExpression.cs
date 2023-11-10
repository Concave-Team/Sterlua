using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Parser.Expressions
{
    public class FunctionCallExpression : Expression
    {
        public string FnName;
        public List<Expression> Parameters;

        public FunctionCallExpression(string fnName, List<Expression> parameters)
        {
            FnName = fnName;
            Parameters = parameters;
        }
    }
}
