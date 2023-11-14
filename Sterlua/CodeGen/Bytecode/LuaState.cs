using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sterlua.CodeGen.Bytecode.STDLib;

namespace Sterlua.CodeGen.Bytecode
{
    public enum BCOpcode
    {
        NOP,
        MOVE,
        LOADBOOL,
        LOADNUM,
        LOADSTR,
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
        public object Operand1;
        public object Operand2;
        public object Operand3;
        public object Operand4;
        public object Operand5;
        public object Operand6;

        public BCInstruction(BCOpcode opcode = BCOpcode.NOP, object operand1 = null, object operand2 = null, object operand3 = null) : this()
        {
            this.opcode = opcode;
            Operand1 = operand1;
            Operand2 = operand2;
            Operand3 = operand3;
        }
    }

    public class LuaStackFrame
    {
        public List<object> Variables;
        public int ReturnAddress;

        public LuaStackFrame(List<object> variables, int returnAddress)
        {
            Variables = variables;
            ReturnAddress = returnAddress;
        }

        public LuaStackFrame() { }
    }

    public class LuaState
    {
        public Stack<object> stack;
        public Stack<LuaStackFrame> callStack;
        public Dictionary<string, object> globals;
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

        public void ExecuteInstructions(List<BCInstruction> insts)
        {
            foreach(var inst in insts)
            {

                PC++;
            }
        }

        public LuaState()
        {
            stack = new Stack<object>();
            globals = new Dictionary<string, object>();
            interopFunctions = new Dictionary<string, Action<object>>
            {
                { "print", new Action<object>(StandardLuaFunctions.print) }
            };
        }
    }
}
