using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Lexer
{
    public struct SLRange
    {
        public int StartIndex, StartLine, EndIndex, EndLine;

        public override string ToString()
        {
            return "From line " + StartLine + "; column " + StartIndex + " to line " + EndLine + "; column " + EndIndex;
        }

        public SLRange(int startIndex, int startLine) : this()
        {
            StartIndex = startIndex;
            StartLine = startLine;
        }

        public SLRange(int startIndex, int startLine, int endIndex, int endLine) : this(startIndex, startLine)
        {
            EndIndex = endIndex;
            EndLine = endLine;
        }
    }

    public struct Token
    {
        public TokenType Type;
        public object Value;
        public SLRange Location = new SLRange();

        public Token(TokenType type, object value)
        {
            Type = type;
            Value = value;
        }

        public Token(TokenType type, object value, SLRange location) : this(type, value)
        {
            Location = location;
        }
    }

    /*
         KEYWORDS (prefixed with k_) :
         and       break     do        else      elseif
         end       false     for       function  if
         in        local     nil       not       or
         repeat    return    then      true      until
         while     imvar     require   ext

         Other Tokens(o_):
         symbol, number, string, identifier, operator
     */
    public enum TokenType
    {
        None,
        Unknown,
        EOF,
        k_and,
        k_or,
        k_break,
        k_continue,
        k_end,
        k_repeat,
        k_in,
        k_nil,
        k_not,
        k_if,
        k_else,
        k_elseif,
        k_function,
        k_do,
        k_while,
        k_until,
        k_true,
        k_false,
        k_for,
        k_local,
        k_ext,
        k_return,
        k_then,
        k_imvar,
        k_require,
        o_symbol,
        o_number,
        o_string,
        o_identifier,
        o_operator
    }
}
