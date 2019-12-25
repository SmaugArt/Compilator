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

    public class NodeLiteral : ExpressionNode
    {
        public NodeLiteral():base() { }
        public override string NodeText() => "LiteralNode: "+token.GetText();
        
    }

    //public class NodeBinaryOp : SyntaxisNode
    //{
    //    public NodeBinaryOp() : base() { }
    //    //public SyntaxisNode LeftNode;
    //    //public SyntaxisNode RightNode;
    //    public override string NodeText() => "BinaryOperat: " + token.GetText();
    //}

    //public class NodeUnaryOp : SyntaxisNode
    //{
    //    public NodeUnaryOp() : base() { }
    //    //public SyntaxisNode arg; //аргумент унарных операций
    //    public override string NodeText() => "UnaryOperat:  " + token.GetText();
    //}

    public class NodeIdentificator : ExpressionNode
    {
        public NodeIdentificator():base() { }
        public override string NodeText() => "Identificator:" + token.GetText();
    } //: base("IdentificatorNode") { } }

    ////public class NamespaceNode : ExpressionNode
    ////{
    ////    public NamespaceNode() : base() { }

    ////    public override string NodeText() => "NamespaceNode:" + token.GetText();
    ////}
        
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

    public class SimpleTypeNode : SyntaxisNode
    {
        public SimpleTypeNode() : base() { }

        public override string NodeText() => "SimpleTypeNode:   " + token.GetText();
    }

    public class NullableTypeNode : SimpleTypeNode
    {
        public NullableTypeNode() : base() { }

        public override string NodeText() => "NullableTypeNode:   " + token.GetText();
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

    public class OperatorNode : SyntaxisNode
    {
        public OperatorNode() : base() { }

        public override string NodeText() => "OperatorNode: "+token.GetText();
    }

    public class ExpressionNode : SyntaxisNode
    {
        public ExpressionNode() : base() { }

        public override string NodeText() => "ExpressionNode:   " + token.GetText();
    }

    public class ConditionalExpressionNode : ExpressionNode //:
    {
        public ConditionalExpressionNode() : base() { }

        public override string NodeText() => "ConditionalExpressionNode:" + token.GetText();
    }

    public class ConditionalOrExpressionNode : ExpressionNode //||
    {
        public ConditionalOrExpressionNode() : base() { }

        public override string NodeText() => "ConditionalOrExpressionNode:" + token.GetText();
    }

    public class ConditionalAndExpressionNode : ExpressionNode //&&
    {
        public ConditionalAndExpressionNode() : base() { }

        public override string NodeText() => "ConditionalAndExpressionNode:" + token.GetText();
    }

    public class InclusiveOrExpressionNode : ExpressionNode //|
    {
        public InclusiveOrExpressionNode() : base() { }

        public override string NodeText() => "InclusiveOrExpressionNode:" + token.GetText();
    }
    
    public class ExclusiveOrExpressionNode : ExpressionNode //^
    {
        public ExclusiveOrExpressionNode() : base() { }

        public override string NodeText() => "ExclusiveOrExpressionNode:" + token.GetText();
    }

    public class AndExpressionNode : ExpressionNode //&
    {
        public AndExpressionNode() : base() { }

        public override string NodeText() => "AndExpressionNode:" + token.GetText();
    }

    public class EqualityExpressionNode : ExpressionNode //==, !=
    {
        public EqualityExpressionNode() : base() { }

        public override string NodeText() => "EqualityExpressionNode:" + token.GetText();
    }
    
    public class RelationalExpressionNode : ExpressionNode //Is,As, >=, <=, >, <
    {
        public RelationalExpressionNode() : base() { }

        public override string NodeText() => "RelationalExpressionNode:" + token.GetText();
    }

    public class BinaryOperationExpressionNode : ExpressionNode //Is,As, >=, <=, >, <
    {
        public BinaryOperationExpressionNode() : base() { }

        public override string NodeText() => "BinaryOperationExpressionNode:" + token.GetText();
    }

    public class UnaryOperationExpressionNode : ExpressionNode //Is,As, >=, <=, >, <
    {
        public UnaryOperationExpressionNode() : base() { }

        public override string NodeText() => "UnaryOperationExpressionNode:" + token.GetText();
    }

    public class ObjectCreationExpressionNode : ExpressionNode
    {
        public ObjectCreationExpressionNode() : base() { }

        public override string NodeText() => "OCExpressionNode:   " + token.GetText();
    }

    public class CastEspression : ExpressionNode
    {
        public CastEspression() : base() { }

        public override string NodeText() => "CastEspression:   " + token.GetText();
    }

    public class AssignmentNode : ExpressionNode
    {
        public AssignmentNode() : base() { }

        public override string NodeText() => "AssignmentNode:   " + token.GetText();
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

    public class ArrayTypeNode : SyntaxisNode
    {
        public ArrayTypeNode() : base() { }

        public override string NodeText() => "ArrayTypeNode: []";
    }

    public class ObjectInitializerNode : SyntaxisNode
    {
        public ObjectInitializerNode() : base() { }

        public override string NodeText() => "ArrayTypeNode: {}";
    }

    public class MemberInitializerListNode : SyntaxisNode
    {
        public MemberInitializerListNode() : base() { }

        public override string NodeText() => "ObjectInitializerNode:" + token.GetText();
    }

    public class MemberInitializerNode : SyntaxisNode
    {

        public MemberInitializerNode() : base() { }

        public override string NodeText() => "MemberInitializerNode:" + token.GetText();
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
