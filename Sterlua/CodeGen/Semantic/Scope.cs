using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Semantic
{
    public class Scope
    {
        public List<Symbol> Symbols = new List<Symbol>();

        /// <summary>
        /// Returns a symbol based on name or null, if it doesn't exist.
        /// </summary>
        /// <param name="name">The name of the symbol</param>
        /// <returns></returns>
        public Symbol? GetSymbol(string name)
        {
            return Symbols.FirstOrDefault(e => e.Name == name);
        }

        public void AddSymbol(Symbol symbol)
        {
            if (!SymbolExists(symbol.Name))
            {
                Symbols.Add(symbol);
            }
            else
            {
                throw new BinderException("E3152: attempted to create symbol of duplicate name.");
            }
        }

        public bool SymbolExists(string name) => Symbols.Exists(e => e.Name == name);
    }
}
