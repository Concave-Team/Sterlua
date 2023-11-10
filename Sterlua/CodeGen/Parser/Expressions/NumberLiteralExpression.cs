using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Parser.Expressions
{
    public class NumberLiteralExpression : Expression
    {
        public int Literal;

        public NumberLiteralExpression(int literal)
        {
            Literal = literal;
        }
    }
}
