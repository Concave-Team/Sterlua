using Sterlua.CodeGen.Parser;
using Sterlua.CodeGen.Parser.Expressions;
using Sterlua.CodeGen.Parser.Statements;
using Sterlua.CodeGen.Semantic.BAST.Expressions;
using Sterlua.CodeGen.Semantic.BAST.Statements;
using Sterlua.CodeGen.Semantic.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic
{
    public class Binder
    {
        public Scope GlobalScope = new Scope(); // basically anything that is not scoped, gets placed here.
        public Stack<Scope> Scopes = new Stack<Scope>();
        public Scope CurrentScope;
        public List<BStatement> BStatements = new List<BStatement>();

        public bool SymbolExists(string Name) => (CurrentScope.SymbolExists(Name)) || (GlobalScope.SymbolExists(Name));

        public FunctionSymbol CreateFSymbol(string name, IdentifierType returnType, List<Argument> args)
        {
            return new FunctionSymbol(name, returnType, args);
        }

        // this is so these global functions can resolve to something, and don't throw a brick in your face about them not existing.
        public void GlobalDefaults()
        {
            GlobalScope.AddSymbol(CreateFSymbol("print", IdentifierType.Void, new List<Argument> { new Argument(IdentifierType.String, "value") } ));
        }

        public void PushScope(Scope scope)
        {
            Scopes.Push(scope);
            CurrentScope = scope;
        }

        public void PopScope()
        {
            Scopes.Pop();
            CurrentScope = Scopes.Peek();
        }

        public IdentifierType GetType(Expression value) => value switch
        {
            NumberLiteralExpression => IdentifierType.Number,
            StringLiteralExpression => IdentifierType.String,
            BooleanLiteralExpression => IdentifierType.Bool,
            FunctionCallExpression => GetType(value as FunctionCallExpression),
            _ => IdentifierType.Void
        };

        public IdentifierType GetType(FunctionCallExpression expr)
        {
            if (SymbolExists(expr.FnName))
            {
                var fnSym = GlobalScope.GetSymbol(expr.FnName) as FunctionSymbol;
                return fnSym.ReturnType;
            }

            return IdentifierType.Void;
        }

        public object BindExpression(FunctionCallExpression expr)
        {
            if(SymbolExists(expr.FnName))
            {
                // type-checking, woo--

                var fnSym = GlobalScope.GetSymbol(expr.FnName) as FunctionSymbol; // functions are always global anyway

                if(fnSym != null)
                {
                    int i = 0;
                    foreach(var arg in fnSym.Arguments)
                    {
                        if (arg.ArgType.Name != GetType(expr.Parameters[i]).Name)
                        {
                            throw new BinderException("E3752: type mismatch, expected type '" + arg.ArgType.Name + "' got type '" + GetType(expr.Parameters[i]).Name + "'.");
                        }
                        i++;
                    }

                    var nParams = new List<object>();

                    foreach(var p in expr.Parameters)
                    {
                        nParams.Add(BindExpression(p));
                    }

                    return new BCallFunctionExpression(fnSym, nParams);
                }
                else
                {
                    throw new BinderException("E3050: could not get function symbol (possibly the interpreter's issue) '" + expr.FnName + "'.");
                }
            }
            else
            {
                throw new BinderException("E3052: attempted to call non-existing function '" + expr.FnName + "'.");
            }
        }

        public object BindExpression(Expression expr) => expr switch
        {
            FunctionCallExpression => BindExpression(expr as FunctionCallExpression),
            _ => expr
        };

        public List<BStatement> BindStatements(List<Statement> statements)
        {
            List<BStatement> bstmts = new List<BStatement>();
            foreach (var stmt in statements)
            {
                switch (stmt)
                {
                    case VariableCreateStatement:
                        var cstmt = stmt as VariableCreateStatement;
                        if (!SymbolExists(cstmt.Name))
                        {
                            VariableSymbol sym = new VariableSymbol(cstmt.Name, cstmt.VariableType, null);
                            var ex = BindExpression(cstmt.Value);

                            if(ex is BCallFunctionExpression)
                            {
                                var xx = ex as BCallFunctionExpression;
                                if(xx.symbol.ReturnType.Name != sym.Type.Name)
                                    throw new BinderException("E3752: type mismatch, expected type '" + sym.Type.Name + "' got type '" + xx.symbol.ReturnType.Name + "'.");
                            }

                            sym.PrimaryValue = ex;

                            CurrentScope.AddSymbol(sym);
                            bstmts.Add(new BVariableCreateStatement(sym));
                        }
                        else
                            throw new BinderException("E3652: attempted to redeclare variable '"+cstmt.Name+"'.");
                        break;
                    case FunctionStatement:
                        var fstmt = stmt as FunctionStatement;
                        if (!SymbolExists(fstmt.Name))
                        {
                            FunctionSymbol sym = new FunctionSymbol(fstmt.Name, fstmt.ReturnType, fstmt.Arguments);
                            if (CurrentScope == GlobalScope)
                                CurrentScope.AddSymbol(sym);
                            else
                                throw new BinderException("E3152: cannot declare function in a non-global scope.");

                            CurrentScope = new Scope();
                            Scopes.Push(CurrentScope);
                            var bst = BindStatements(fstmt.Body.statements);
                            Scopes.Pop();

                            bstmts.Add(new BFunctionStatement(sym, new BBlockStatement(bst)));
                        }
                        else
                            // TO-DO: do something else when overloads are added(if ever.)
                            throw new BinderException("E3652: attempted to redeclare function '" + fstmt.Name + "'.");
                        break;
                    case ExpressionStatement:
                        var expr = stmt as ExpressionStatement;
                        bstmts.Add(new BExpressionStatement(BindExpression(expr.expr) as BExpression));
                        break;
                }
            }

            return bstmts;
        }

        public List<BStatement> Run(List<Statement> statements)
        {
            PushScope(GlobalScope);

            GlobalDefaults();

            BStatements = BindStatements(statements);

            return BStatements;
        }
    }

    public class BinderException : Exception
    {
        public BinderException()
        {
        }

        public BinderException(string message)
            : base(message)
        {
        }

        public BinderException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
