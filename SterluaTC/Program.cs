using Sterlua.CodeGen.Lexer;
using Sterlua.CodeGen.Parser;
using Sterlua.CodeGen.Semantic;
using System.Runtime.CompilerServices;

namespace SterluaTC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                SLLexer lexer = new SLLexer(File.ReadAllText(args[0]));
                var toks = lexer.Run();
                Console.WriteLine(toks.Count);
                foreach (var item in toks)
                {
                    Console.WriteLine(item.Type + ": " + item.Value + " Location: from line " + item.Location.StartLine + "; column " + item.Location.StartIndex + " to line " + item.Location.EndLine + "; column " + item.Location.EndIndex + ".");
                }

                Parser parser = new Parser();
                var statements = parser.Run(toks);
                Console.WriteLine(statements.Count);
                var sem = new Binder().Run(statements);
            }
        }
    }
}