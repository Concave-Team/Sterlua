using Sterlua.CodeGen.Bytecode;
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
                //var sem = new Binder().Run(statements);

                LuaState stateTest = new LuaState();

                /*
                    CALL print "Function Test Initialized"
                    
                    LABEL testfn
                    call print "This is from the function!"
                    RETURN VOID

                    CALL testfn
                 */
                // My special bytecode:

                LuaFunction testfn = new LuaFunction();
                testfn.Instructions = new List<BCInstruction> { new BCInstruction(BCOpcode.CALL, new List<object> { "print", "This is from the function!" }), new BCInstruction(BCOpcode.RETURNVOID) };
                testfn.Name = "testfn";

                stateTest.functions.Add("testfn", testfn);

                List<BCInstruction> instrcts = new List<BCInstruction>
                {
                    new BCInstruction(BCOpcode.CALL, new List<object> {"print", "Function Test Initialized!"}),
                    new BCInstruction(BCOpcode.CALL, new List<object> {"testfn"})
                };

                stateTest.ExecuteInstructions(instrcts);
            }
        }
    }
}