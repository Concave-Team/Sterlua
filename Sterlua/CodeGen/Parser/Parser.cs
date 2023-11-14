using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sterlua.CodeGen.Lexer;
using Sterlua.CodeGen.Parser.Expressions;
using Sterlua.CodeGen.Parser.Statements;

namespace Sterlua.CodeGen.Parser
{
    public class Parser
    {
        public List<Token> Tokens;
        public List<Statement> Statements = new List<Statement>();
        public Token CurrentToken;
        public int index;

        public void Next()
        {
            index++;
            if(index < Tokens.Count)
            {
                CurrentToken = Tokens[index];
            }
        }
        public Token Consume(TokenType type)
        {
            if(CurrentToken.Type == type)
            {
                var crt = CurrentToken;
                Next(); return crt;
            }
            throw new ParserException("Expected '" + type + "', got '" + CurrentToken.Type + ". "+ CurrentToken.Location.ToString());
        }

        public Token ConsumeWithValue(TokenType type, object value)
        {
            Console.WriteLine(type + " " + CurrentToken.Type);
            if (value.GetType().IsAssignableFrom(CurrentToken.Value.GetType()))
            {
                if (CurrentToken.Type == type && CurrentToken.Value.Equals(value))
                {
                    var crt = CurrentToken;
                    Next(); return crt;
                }
            }
            else
                // CTE-5150 means Compile-Time Error - code 5150(Section - Parser(51), Severity - Critical, Fault - Interpreter)
                throw new ParserException("CTE-5150: token type/check value mismatch - this might indicate a problem with the interpreter. "+CurrentToken.Location.ToString());
            // CTE-5152 means Compile-Time Error - code 5152(Section - Parser(51), Severity - Critical, Fault - User Error)
            throw new ParserException("CTE-5152: Expected '" + type + "' (" + value + "), got '" + CurrentToken.Type + "' ("+ CurrentToken.Value + "). " + CurrentToken.Location.ToString()); ;
        }

        public Token Peek(int n)
        {
            if (index+n < Tokens.Count)
            {
                return Tokens[index+n];
            }
            throw new ParserException("Could not peek by " + n + " steps. (Index Out Of Bounds)");
        }

        public NumberLiteralExpression ParseNumLiteral()
        {
            var num = Consume(TokenType.o_number);
            return new NumberLiteralExpression((int)num.Value);
        }
        public StringLiteralExpression ParseStrLiteral()
        {
            var num = Consume(TokenType.o_string);
            return new StringLiteralExpression((string)num.Value);
        }
        // o_identifier '(' {expr, expr}? ')'
        public FunctionCallExpression ParseFnCall()
        {
            var NameIdent = Consume(TokenType.o_identifier);
            ConsumeWithValue(TokenType.o_symbol, '(');

            // run expression list until ) or <eof>

            List<Expression> ParameterList = new List<Expression>();

            // '(' {expr, expr}? ')'
            while(!CurrentToken.Value.Equals(')') && CurrentToken.Type != TokenType.EOF)
            {
                Console.WriteLine("FC: " + CurrentToken.Value.Equals(')'));
                ParameterList.Add(ParseExpression());

                if (CurrentToken.Value.Equals(','))
                {
                    ConsumeWithValue(TokenType.o_symbol, ',');
                }
            }

            ConsumeWithValue(TokenType.o_symbol, ')');
            return new FunctionCallExpression((string)NameIdent.Value, ParameterList);
        }

        public VariableExpression ParseVariableExpr()
        {
            var NameIdent = Consume(TokenType.o_identifier);
            return new VariableExpression((string)NameIdent.Value);
        }

        // expr_stmt: expr
        public ExpressionStatement ParseExprStmt()
        {
            var expr = ParseExpression();
            return new ExpressionStatement(expr);
        }

        // REFERENCE RULE: var ::= k_local? k_imvar? o_identifier ':' o_identifier '=' expr
        public VariableCreateStatement ParseVariableCreate()
        {
            Accessibility aMod = Accessibility.Global;
            if(CurrentToken.Type == TokenType.k_local)
            {
                // The Local Modifier is Optional.
                Consume(TokenType.k_local);
                aMod = Accessibility.Local;
            }

            bool isImvar = false;
            if (CurrentToken.Type == TokenType.k_imvar)
            {
                // The Local Modifier is Optional.
                Consume(TokenType.k_imvar);
                isImvar = true;
            }

            var NameIdent = Consume(TokenType.o_identifier); // Name identifier token.
            IdentifierType _Type = null;
            if(!isImvar)
            {
                ConsumeWithValue(TokenType.o_symbol, ':');
                var TypeIdent = Consume(TokenType.o_identifier);
                _Type = new IdentifierType((string)TypeIdent.Value); // do type stuff if theres no imvar, :)
            }

            ConsumeWithValue(TokenType.o_operator, "=");

            var ValueExpr = ParseExpression();

            Console.WriteLine("now: "+CurrentToken.Type);
            
            return new VariableCreateStatement(aMod, (string)NameIdent.Value, isImvar, _Type, ValueExpr);
        }

        // o_identifier ':' type
        public Argument ParseArg()
        {
            var NameIdent = Consume(TokenType.o_identifier);
            ConsumeWithValue(TokenType.o_symbol, ':');
            var TypeIdent = Consume(TokenType.o_identifier);

            return new Argument(new IdentifierType((string)TypeIdent.Value), (string)NameIdent.Value);
        }

        public BlockStatement ParseBlockStmt()
        {
            List<Statement> stmts = new List<Statement>();
            while(CurrentToken.Type is not TokenType.k_end)
            {
                stmts.Add(ParseStatement());
            }
            Consume(TokenType.k_end);
            return new BlockStatement(stmts);
        }

        // k_function o_identifier '(' {arg, arg}? ')' ':' type block_stmt
        public FunctionStatement ParseFn()
        {
            Consume(TokenType.k_function);
            
            var NameIdent = Consume(TokenType.o_identifier);
            Console.WriteLine("A: "+CurrentToken.Type);
            ConsumeWithValue(TokenType.o_symbol, '(');

            List<Argument> args = new List<Argument>();

            // '(' {arg, arg}? ')'
            while (!CurrentToken.Value.Equals(')') && CurrentToken.Type != TokenType.EOF)
            {
                args.Add(ParseArg());

                if (CurrentToken.Value.Equals(','))
                {
                    ConsumeWithValue(TokenType.o_symbol, ',');
                }
            }

            ConsumeWithValue(TokenType.o_symbol, ')');
            ConsumeWithValue(TokenType.o_symbol, ':');

            var TypeIdent = Consume(TokenType.o_identifier);

            BlockStatement bstmt = ParseBlockStmt();

            return new FunctionStatement((string)NameIdent.Value, bstmt, new IdentifierType((string)TypeIdent.Value), args);
        }



        public Expression ParseExpression()
        {  
            switch(CurrentToken.Type)
            {
                case TokenType.o_number:
                    return ParseNumLiteral();
                case TokenType.o_string:
                    return ParseStrLiteral();
                case TokenType.o_identifier:
                    if (Peek(-1).Type != TokenType.k_function)
                    {
                        var pTok = Peek(1);
                        if (pTok.Type == TokenType.o_symbol && pTok.Value.Equals('('))
                            return ParseFnCall();
                        else
                            return ParseVariableExpr();
                    }
                    break;
            }

            throw new ParserException("expression missing. "+CurrentToken.Location.ToString());
        }

        public ReturnStatement ParseReturn()
        {
            Consume(TokenType.k_return);
            return new ReturnStatement(ParseExpression());
        }

        public Statement ParseStatement()
        {
            switch (CurrentToken.Type)
            {
                // CTX - /local/ var.
                case TokenType.k_local:
                    return ParseVariableCreate();
                case TokenType.k_function:
                    Statements.Add(ParseFn());
                    break;
                case TokenType.o_identifier:
                    // CTXs - /< >/ identifier var; /identifier/(...) call/
                    // if =, then - CTX = var, if ( - CTX = call.

                    var pTok = Peek(1);
                    if (pTok.Type == TokenType.o_symbol && pTok.Value.Equals('='))
                    {
                        return ParseVariableCreate();
                    }
                    else // considering there are no other grammar rules, we can just assume that the else is going to be a function call.
                    {
                        return ParseExprStmt();
                    }
                case TokenType.k_return:
                    return ParseReturn();

            }

            throw new ParserException("couldn't parse statement. " + CurrentToken.Location.ToString());
        }

        /// <summary>
        /// Creates an AST from the list of tokens.
        /// </summary>
        /// <param name="tokens">The List of Tokens to process.</param>
        /// <returns></returns>
        public List<Statement> Run(List<Token> tokens)
        {
            Tokens = tokens;
            index = 0;
            CurrentToken = Tokens[index];

            while(CurrentToken.Type != TokenType.EOF)
            {
                Console.WriteLine(CurrentToken.Type);
                switch (CurrentToken.Type)
                {
                    // CTX - /local/ var.
                    case TokenType.k_local:
                        Statements.Add(ParseVariableCreate());
                        continue;
                    case TokenType.k_function:
                        Statements.Add(ParseFn());
                        continue;
                    case TokenType.o_identifier:
                        // CTXs - /< >/ identifier var; /identifier/(...) call/
                        // if =, then - CTX = var, if ( - CTX = call.

                        var pTok = Peek(1);
                        if(pTok.Type == TokenType.o_symbol && pTok.Value.Equals('='))
                        {
                            Statements.Add(ParseVariableCreate());
                            break;
                        }
                        else // considering there are no other grammar rules, we can just assume that the else is going to be a function call.
                        {
                            Statements.Add(ParseExprStmt());
                        }

                        break;

                }
                Next();
            }

            return Statements;
        }
    }

    public class IdentifierType
    {
        public string Name;

        public static IdentifierType Number => new IdentifierType("number");
        public static IdentifierType String => new IdentifierType("string");
        public static IdentifierType Bool => new IdentifierType("bool");
        public static IdentifierType Void => new IdentifierType("void");

        public IdentifierType(string name)
        {
            Name = name;
        }
    }

    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message)
            : base(message)
        {
        }

        public ParserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
