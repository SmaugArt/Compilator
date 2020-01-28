using Compilator.SyntaxisModule.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilator.SymanticModule
{
    public enum SymExType
    {
        NonType,
        IncorrectNode,
        SimpleIdentify,
        NonexistentIdentify,

    }
    public static class SymException
    {
        public static Exception Show(SymExType type, SyntaxisNode node)
        {
            switch (type)
            {
                case SymExType.IncorrectNode:
                    return new Exception("Unexpected node type: "+node.GetType()+"!");
                case SymExType.SimpleIdentify:
                    return new Exception("This identify is already in use: "+node.token.ToString());
                case SymExType.NonexistentIdentify:
                    return new Exception("This identify is not exist: "+node.token.ToString());
                default:
                    return new Exception("Exception of checking a node: "+ node.ToString() + "; Position: "+node.token.ToString());
            }
        }
    }
}
