using Compilator.GeneralStructures;
using System.Collections.Generic;

namespace Compilator.SyntaxisModule.Structures
{
    public abstract class SyntaxisNode
    {
        public Token token;
        //public SyntaxisNode parent;
        public List<SyntaxisNode> children;
        public SyntaxisNode() { children = new List<SyntaxisNode>();}

        public override string ToString() => PrintMethod(0, this, true);//=> token.GetText();
        public abstract string NodeText();
        private string PrintMethod(int level, SyntaxisNode node, bool endLevelElement)
        {
            string newStr = "";
            SetOffset(ref newStr, level, '▓');

            newStr += (endLevelElement) ? '˪' : '˫';
            newStr += node.NodeText()+ "\r\n";//node.token.GetText()+"\r\n";
            for (int i = 0; i < node.children.Count; i++)
            {
                newStr += (i + 1 == node.children.Count) ? PrintMethod(level + 1, node.children[i], true) : 
                    PrintMethod(level + 1, node.children[i], false);
            }

            return newStr;
        }

        private void SetOffset(ref string str, int valOffset, char sym)
        {
            for (int i = 0; i < valOffset;i++)
                str += sym;
        }
    }

    public class NodeLiteral : SyntaxisNode
    {
        public NodeLiteral():base() { }
        public override string NodeText() => "LiteralNode: "+token.GetText();
        
    }

    public class NodeBinaryOp : SyntaxisNode
    {
        public NodeBinaryOp() : base() { }
        //public SyntaxisNode LeftNode;
        //public SyntaxisNode RightNode;
        public override string NodeText() => "BinaryOperat: " + token.GetText();
    }

    public class NodeUnaryOp : SyntaxisNode
    {
        public NodeUnaryOp() : base() { }
        //public SyntaxisNode arg; //аргумент унарных операций
        public override string NodeText() => "UnaryOperat:  " + token.GetText();
    }

    public class NodeIdentificator : SyntaxisNode
    {
        public NodeIdentificator():base() { }
        public override string NodeText() => "Identificator:" + token.GetText();
    } //: base("IdentificatorNode") { } }
    public class NodeChar : NodeLiteral
    {
        public NodeChar() : base() { }
        public override string NodeText() => "CharNode:     " + token.GetText();
    }
    public class NodeDouble : NodeLiteral
    {
        public NodeDouble() : base() { }
        public override string NodeText() => "DoubleNode:   " + token.GetText();
    }
    public class NodeInt : NodeLiteral
    {
        public NodeInt() : base() { }

        public override string NodeText() => "IntegerNode:  " + token.GetText();
    }
    public class NodeString : NodeLiteral
    {
        public NodeString() : base() { }
        public override string NodeText() => "StringNode:   " + token.GetText();
    }

    public class NodeBool : NodeLiteral
    {
        public NodeBool() : base() { }
        public override string NodeText() => "BoolNode:   " + token.GetText();
    }

    public class NullNode : NodeLiteral
    {
        public NullNode() : base() { }
        public override string NodeText() => "NullNode:   " + token.GetText();
    }

    /// <summary>
    /// Необходимый нод для обозначения конца строки
    /// </summary>
    public class EmptyNode : SyntaxisNode
    {
        public EmptyNode() : base() { }

        public override string NodeText() => "Empty node! Lexer reached the end of the line.";
    }

    public class ExpressionNode : SyntaxisNode
    {
        public ExpressionNode() : base() { }

        public override string NodeText() => "ExpressionNode:   " + token.GetText();
    }

    public class ObjectCreationExpressionNode : SyntaxisNode
    {
        public ObjectCreationExpressionNode() : base() { }

        public override string NodeText() => "OCExpressionNode:   " + token.GetText();
    }

    public class MemberAccessNode : SyntaxisNode
    {
        public MemberAccessNode() : base() { }

        public override string NodeText() => "MemberAccessNode:   " + token.GetText();
    }

    public class PostIncrementNode : SyntaxisNode
    {
        public PostIncrementNode() : base() { }

        public override string NodeText() => "PostIncrementNode:   " + token.GetText();
    }

    public class PostDecrementNode : SyntaxisNode
    {
        public PostDecrementNode() : base() { }

        public override string NodeText() => "PostDecrementNode:   " + token.GetText();
    }

    public class ElementAccessNode : SyntaxisNode
    {
        public ElementAccessNode() : base() { }

        public override string NodeText() => "ElementAccessNode:  []";
    }

    public class InvocationExpressionNode : SyntaxisNode
    {
        public InvocationExpressionNode() : base() { }

        public override string NodeText() => "InvocationExpressionNode: ()";
    }






    public class GlobalNode : SyntaxisNode //содержит using, class и структуры enum/struct
    {
        public GlobalNode() : base() { }

        public override string NodeText() => " ";
    }

    public class UsingNode : SyntaxisNode
    {
        public UsingNode() : base() { }
        public override string NodeText() => "UsingNode";
    }





    //public class EnumNode : SyntaxisNode { }

    //public class StructureNode : SyntaxisNode { }

    //public class ClassNode : SyntaxisNode { }

    //public class IntegerPeremNode : SyntaxisNode { }  //содержит имя переменной, где слева - modificator(public, static, private) //without overide/abstract/virtual /// + int

    //public class IntegerPeremNode : SyntaxisNode { }

}
