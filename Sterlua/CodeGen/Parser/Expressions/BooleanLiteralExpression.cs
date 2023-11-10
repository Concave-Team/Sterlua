using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Parser.Expressions
{
    public class BooleanLiteralExpression : Expression
    {
        public bool Literal;

        public BooleanLiteralExpression(bool literal)
        {
            Literal = literal;
        }
    }
}
