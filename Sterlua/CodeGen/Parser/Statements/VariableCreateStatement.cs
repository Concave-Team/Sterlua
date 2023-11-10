using Sterlua.CodeGen.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sterlua.CodeGen.Parser.Statements
{
    public enum Accessibility
    {
        Global,
        Local
    }
    public class VariableCreateStatement : Statement
    {
        public Accessibility AccessModifier;
        public string Name;
        public bool IsImplicit;
        public IdentifierType VariableType;
        public Expression Value;

        public VariableCreateStatement(Accessibility accessModifier, string name, bool isImplicit, IdentifierType variableType, Expression value)
        {
            AccessModifier = accessModifier;
            Name = name;
            IsImplicit = isImplicit;
            VariableType = variableType;
            Value = value;
        }
    }
}
