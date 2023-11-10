using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Parser.Expressions
{
    public class StringLiteralExpression : Expression
    {
        public string Literal;

        public StringLiteralExpression(string literal)
        {
            Literal = literal;
        }
    }
}
