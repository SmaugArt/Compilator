using Compilator.GeneralStructures;

namespace Compilator.SyntaxisModule.Structures.AbstractStructures
{
    public abstract class SyntaxisNode
    {
        public Token token;
    }

    public class NodeLiteral : SyntaxisNode{ }

    public class NodeBinaryOp : SyntaxisNode
    {
        public SyntaxisNode LeftNode;
        public SyntaxisNode RightNode;
    }

    public class NodeUnaryOp : SyntaxisNode
    {
        public SyntaxisNode arg; //аргумент унарных операций
    }

    public class NodeIdentificator : SyntaxisNode { }
    public class NodeChar : NodeLiteral { }
    public class NodeDouble : NodeLiteral { }
    public class NodeInt : NodeLiteral { }
    public class NodeString : NodeLiteral { }

}
