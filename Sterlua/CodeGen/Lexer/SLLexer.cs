using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Lexer
{
    public class SLLexer
    {
        public List<Token> ETokenStream = new List<Token>();
        public string CharStream;
        public int index, col, line;
        public char CurrentChar;

        public void Next()
        {
            index++;
            if (index < CharStream.Length)
            {
                CurrentChar = CharStream[index];
                col++;
            }
        }

        public char Peek(int n)
        {
            if (index + n < CharStream.Length)
            {
                return CharStream[index+n];
            }
            return char.MinValue;
        }

        public Token GetNumber() 
        {
            Token tok = new Token();

            string num = "";

            tok.Location.StartLine = line;
            tok.Location.StartIndex = col;
            
            while(index < CharStream.Length && Char.IsNumber(CurrentChar))
            {
                num += CurrentChar;
                Next();
            }


            tok.Type = TokenType.o_number;
            Console.WriteLine(num);
            tok.Value = int.Parse(num);
            tok.Location.EndLine = line;
            tok.Location.EndIndex = col;
            return tok;
        }

        public Token GetString() 
        {
            Token tok = new Token();

            string str = "";

            tok.Location.StartLine = line;
            tok.Location.StartIndex = col;

            while (index < CharStream.Length && CurrentChar != '\'' && CurrentChar != '"')
            {
                str += CurrentChar;
                Next();
            }


            tok.Type = TokenType.o_string;
            tok.Value = str;
            tok.Location.EndLine = line;
            tok.Location.EndIndex = col;
            return tok;
        }

        public Token GetIdentifier()
        {
            Token tok = new Token();

            string str = "";

            tok.Location.StartLine = line;
            tok.Location.StartIndex = col;

            while (index < CharStream.Length && char.IsLetterOrDigit(CurrentChar))
            {
                str += CurrentChar;
                Next();
            }


            /*KEYWORDS (prefixed with k_) :
         and       break     do        else      elseif
         end       false     for       function  if
         in        local     nil       not       or
         repeat    return    then      true      until
         while     imvar     require   ext*/

            switch (str)
            {
                case "and": tok.Type = TokenType.k_and; break;
                case "break": tok.Type = TokenType.k_break; break;
                case "do": tok.Type = TokenType.k_do; break;
                case "else": tok.Type = TokenType.k_else; break;
                case "elseif": tok.Type = TokenType.k_elseif; break;
                case "end": tok.Type = TokenType.k_end; break;
                case "false": tok.Type = TokenType.k_false; break;
                case "for": tok.Type = TokenType.k_for; break;
                case "function": tok.Type = TokenType.k_function; break;
                case "if": tok.Type = TokenType.k_if; break;
                case "in": tok.Type = TokenType.k_in; break;
                case "local": tok.Type = TokenType.k_local; break;
                case "nil": tok.Type = TokenType.k_nil; break;
                case "not": tok.Type = TokenType.k_not; break;
                case "or": tok.Type = TokenType.k_or; break;
                case "repeat": tok.Type = TokenType.k_repeat; break;
                case "return": tok.Type = TokenType.k_return; break;
                case "then": tok.Type = TokenType.k_then; break;
                case "true": tok.Type = TokenType.k_true; break;
                case "until": tok.Type = TokenType.k_until; break;
                case "while": tok.Type = TokenType.k_while; break;
                case "imvar": tok.Type = TokenType.k_imvar; break;
                case "require": tok.Type = TokenType.k_require; break;
                case "ext": tok.Type = TokenType.k_ext; break;
                default: tok.Type = TokenType.o_identifier; break;
            }   
            tok.Value = str;

            tok.Location.EndLine = line;
            tok.Location.EndIndex = col;
            return tok;
        }

        public Token GetOperator() 
        { 
            switch(Peek(1))
            {
                case '=':
                    switch(CurrentChar)
                    {
                        // >=
                        case '>':
                            Next();
                            return new Token(TokenType.o_operator, ">=", new SLRange(col, line, col+1, line));
                        // <=
                        case '<':
                            Next();
                            return new Token(TokenType.o_operator, "<=", new SLRange(col, line, col + 1, line));
                        // ==
                        case '=':
                            Next();
                            return new Token(TokenType.o_operator, "==", new SLRange(col, line, col +1, line));
                        // ~=
                        case '~':
                            Next();
                            return new Token(TokenType.o_operator, "~=", new SLRange(col, line, col + 1, line));
                    }
                    break;
                default: break;
            }
            switch (CurrentChar)
            {
                // >
                case '>':
                    return new Token(TokenType.o_operator, ">", new SLRange(col, line, col + 1, line));
                // <
                case '<':
                    return new Token(TokenType.o_operator, "<", new SLRange(col, line, col + 1, line));
                // =
                case '=':
                    return new Token(TokenType.o_operator, "=", new SLRange(col, line, col + 1, line));
                // ~
                case '~':
                    return new Token(TokenType.o_operator, "~", new SLRange(col, line, col + 1, line));
            }

            return new Token(TokenType.Unknown, CurrentChar);
        }

        public List<Token> Run()
        {
            this.ETokenStream = new List<Token>();
            index = 0;
            line = 1;

            CurrentChar = CharStream[index];
            //Console.WriteLine("Processing code:\n" + CharStream);

            while(index < CharStream.Length)
            {
                if (char.IsNumber(CurrentChar))
                {
                    ETokenStream.Add(GetNumber());
                    continue;
                }
                else if (CurrentChar == '"' || CurrentChar == '\'')
                {
                    Next();
                    ETokenStream.Add(GetString());
                    goto finish;
                }
                else if (CurrentChar == '_' || char.IsLetter(CurrentChar))
                {
                    ETokenStream.Add(GetIdentifier());
                    continue;
                }
                else if (char.IsWhiteSpace(CurrentChar) || CurrentChar == ' ')
                {
                    if (CurrentChar == '\n')
                    {
                        line++;
                        col = 0;
                    }
                    goto finish;
                }
                else
                {
                    var tok = GetOperator();
                    if (tok.Type == TokenType.Unknown)
                    {
                        if (char.IsSymbol(CurrentChar) || char.IsPunctuation(CurrentChar) || CurrentChar == '(' || CurrentChar == ')')
                        {
                            tok = new Token(TokenType.o_symbol, CurrentChar, new SLRange(col, line, col+1, line));
                        }
                    }

                    ETokenStream.Add(tok);
                    goto finish;
                }

            finish: // just to annoy y'all
                //Console.WriteLine($"Current Char: {CurrentChar} Location: at line {line}; column {col}.");
                //Console.WriteLine($"Last Token Data: {ETokenStream.Last().Type}: {ETokenStream.Last().Value}");
                Next();
            }

            ETokenStream.Add(new Token(TokenType.EOF, "<eof>"));

            return ETokenStream;
        }

        public SLLexer(string feed) 
        { 
            CharStream = feed;
        }
    }
}
