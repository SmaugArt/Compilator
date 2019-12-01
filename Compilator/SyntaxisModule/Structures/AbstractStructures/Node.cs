using Compilator.GeneralStructures;

namespace Compilator.SyntaxisModule.Structures.AbstractStructures
{
    public abstract class SyntaxisNode
    {
        public Token token;

        virtual public string Print() => token.GetText();
    }

    public class NodeLiteral : SyntaxisNode { public NodeLiteral() { } } // base("LiteralNode") { }}

    public class NodeBinaryOp : SyntaxisNode
    {
        public SyntaxisNode LeftNode;
        public SyntaxisNode RightNode;

        //public NodeBinaryOp() : base("BinaryOperationNode") { }
    }

    public class NodeUnaryOp : SyntaxisNode
    {
        public SyntaxisNode arg; //аргумент унарных операций

        //public NodeUnaryOp() : base("UnaryOperationNode") { }
    }

    public class NodeIdentificator : SyntaxisNode { public NodeIdentificator() { } } //: base("IdentificatorNode") { } }
    public class NodeChar : NodeLiteral { public NodeChar() : base() { } }
    public class NodeDouble : NodeLiteral { public NodeDouble() : base() { } }
    public class NodeInt : NodeLiteral { public NodeInt() : base() { } }
    public class NodeString : NodeLiteral { public NodeString() : base() { } }

}
