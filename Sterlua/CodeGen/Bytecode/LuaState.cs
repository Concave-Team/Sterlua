using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sterlua.CodeGen.Bytecode.STDLib;

namespace Sterlua.CodeGen.Bytecode
{
    struct NilState
    {
    }
    public enum BCOpcode
    {
        NOP,
        MOVE,
        LOADBOOL,
        LOADNUM,
        LOADSTR,
        REGSTACK,
        LOADNIL,
        ADD,
        SUB,
        MUL,
        DIV,
        MOD,
        POW,
        IDIV,
        CALL,
        JMP,
        RETURN,
        LABEL,
        RETURNVOID
    }

    // <OP-CODE> <...6 operands>?
    public struct BCInstruction
    {
        public BCOpcode opcode;
        public List<object> operands;

        public BCInstruction(BCOpcode opcode = BCOpcode.NOP, List<object> operands = null)
        {
            this.opcode = opcode;
            this.operands = operands;
        }
    }

    public class LuaStackFrame
    {
        public List<object> Variables;

        public LuaStackFrame(List<object> variables)
        {
            Variables = variables;
        }

        public LuaStackFrame() { }
    }

    public struct LuaFunction
    {
        public string Name;
        public List<BCInstruction> Instructions;
    }

    public class LuaState
    {
        public Stack<object> stack;
        public Stack<LuaStackFrame> callStack;
        public object[] Registers = new object[4];
        public Dictionary<string, object> globals;
        public Dictionary<string,  LuaFunction> functions;
        public Dictionary<string, Action<object>> interopFunctions;
        public int PC = 0; // Program Counter - starts from 0.

        public object GetGlobal(string name)
        {
            if (globals.ContainsKey(name))
            {
                return globals[name];
            }

            // Handle undefined global variable error
            throw new KeyNotFoundException($"Global variable '{name}' not found");
        }

        public void SetGlobal(string name, object value)
        {
            globals[name] = value;
        }

        public void ExecuteInstruction(BCInstruction inst)
        {
            switch (inst.opcode)
            {
                case BCOpcode.NOP:
                    break;
                case BCOpcode.MOVE:
                    var regA = (int)inst.operands[0];
                    var regB = (int)inst.operands[1];

                    Registers[regB] = regA;
                    break;
                case BCOpcode.LOADBOOL:
                    var reg = (int)inst.operands[0];
                    var val = (bool)inst.operands[1];

                    Registers[reg] = val;
                    break;
                case BCOpcode.LOADNUM:
                    var regn = (int)inst.operands[0];
                    var valn = (float)inst.operands[1];

                    Registers[regn] = valn;
                    break;
                case BCOpcode.LOADSTR:
                    var regs = (int)inst.operands[0];
                    var vals = (string)inst.operands[1];

                    Registers[regs] = vals;
                    break;
                case BCOpcode.LOADNIL:
                    var regnl = (int)inst.operands[0];

                    Registers[regnl] = new NilState();
                    break;
                case BCOpcode.REGSTACK:
                    var regrs = (int)inst.operands[0];

                    stack.Push(Registers[regrs]);
                    break;
                case BCOpcode.CALL:
                    var cStack = new LuaStackFrame(new List<object>());
                    callStack.Push(cStack);

                    // clear the registers
                    for (int i = 0; i < Registers.Length; i++)
                        Registers[i] = null;

                    var name = (string)inst.operands[0];
                    var arguments = new List<object>(inst.operands);
                    arguments.RemoveAt(0);

                    // put it into the local variables at exact order. The number in order is now the argument vector index(AVI).

                    foreach(var arg in arguments)
                        callStack.Peek().Variables.Add(arg);

                    if (functions.ContainsKey(name))
                    {
                        var fnobj = functions[name];
                        ExecuteInstructions(fnobj.Instructions);
                    }
                    else if (name == "print")
                    {
                        StandardLuaFunctions.print(arguments[0]);
                    }

                    callStack.Pop();
                    break;
                case BCOpcode.RETURN:
                    var regr = (int)inst.operands[0];
                    stack.Push(Registers[regr]);
                    break;
                case BCOpcode.RETURNVOID:
                    break; // basically just a fancy nop
            }
        }

        public void ExecuteInstructions(List<BCInstruction> insts)
        {
            foreach(var inst in insts)
            {
                ExecuteInstruction(inst);
                PC++;
            }
        }

        public LuaState()
        {
            stack = new Stack<object>();
            globals = new Dictionary<string, object>();
            functions = new Dictionary<string, LuaFunction>();
            callStack = new Stack<LuaStackFrame>();

            interopFunctions = new Dictionary<string, Action<object>>
            {
                { "print", new Action<object>(StandardLuaFunctions.print) }
            };
        }
    }
}
