using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Bytecode.STDLib
{
    public static class StandardLuaFunctions
    {
        public static void print(object str)
        {
            Console.WriteLine(str);
        }
    }
}
