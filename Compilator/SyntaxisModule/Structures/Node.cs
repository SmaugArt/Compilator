using Compilator.GeneralStructures;
using System.Collections.Generic;

namespace Compilator.SyntaxisModule.Structures
{
    public abstract class SyntaxisNode
    {
        public Token token;
        //public SyntaxisNode parent;
        public List<SyntaxisNode> children;
        public SyntaxisNode() { children = new List<SyntaxisNode>(); }

        public override string ToString() => PrintMethod(0, this, true);//=> token.GetText();
        public abstract string NodeText();
        private string PrintMethod(int level, SyntaxisNode node, bool endLevelElement)
        {
            string newStr = "";
            SetOffset(ref newStr, level, '▓');

            newStr += (endLevelElement) ? '˪' : '˫';
            newStr += node.NodeText() + "\r\n";//node.token.GetText()+"\r\n";
            for (int i = 0; i < node.children.Count; i++)
            {
                newStr += (i + 1 == node.children.Count) ? PrintMethod(level + 1, node.children[i], true) :
                    PrintMethod(level + 1, node.children[i], false);
            }

            return newStr;
        }

        private void SetOffset(ref string str, int valOffset, char sym)
        {
            for (int i = 0; i < valOffset; i++)
                str += sym;
        }
    }

    public class NodeLiteral : ExpressionNode
    {
        public NodeLiteral() : base() { }
        public override string NodeText() => "LiteralNode: " + token.GetText();

    }

    public class NodeIdentificator : ExpressionNode
    {
        public NodeIdentificator() : base() { }
        public override string NodeText() => "Identificator:" + token.GetText();
    }

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

    #region TypeNode
    public class TypeNode : SyntaxisNode
    {
        public TypeNode() : base() { }

        public override string NodeText() => "Base type node for all types:   " + token.GetText();
    }

    public class SimpleTypeNode : TypeNode
    {
        public SimpleTypeNode() : base() { }

        public override string NodeText() => "SimpleTypeNode:   " + token.GetText();
    }

    public class VoidTypeNode : TypeNode
    {
        public VoidTypeNode() : base() { }

        public override string NodeText() => "VoidTypeNode:   " + token.GetText();
    }

    public class VarTypeNode : TypeNode
    {
        public VarTypeNode() : base() { }

        public override string NodeText() => "VarTypeNode:   " + token.GetText();
    }

    public class NullableTypeNode : TypeNode
    {
        public NullableTypeNode() : base() { }

        public override string NodeText() => "NullableTypeNode:   " + token.GetText();
    }

    public class ArrayTypeNode : TypeNode
    {
        public ArrayTypeNode() : base() { }

        public override string NodeText() => "ArrayTypeNode: []";
    }
    #endregion

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

        public override string NodeText() => "OperatorNode: " + token.GetText();
    }

    #region Expression
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

        public override string NodeText() => "CastEspression:  ()"; //+ token.GetText();
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

    public class PreIncrementNode : SyntaxisNode
    {
        public PreIncrementNode() : base() { }

        public override string NodeText() => "PreIncrementNode:   " + token.GetText();
    }

    public class PreDecrementNode : SyntaxisNode
    {
        public PreDecrementNode() : base() { }

        public override string NodeText() => "PreDecrementNode:   " + token.GetText();
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


    public class ObjectInitializerNode : SyntaxisNode
    {
        public ObjectInitializerNode() : base() { }

        public override string NodeText() => "ObjectInitializerNode: {}";
    }

    public class MemberInitializerNode : SyntaxisNode
    {

        public MemberInitializerNode() : base() { }

        public override string NodeText() => "MemberInitializerNode:" + token.GetText();
    }

    public class CollectionInitializerNode : SyntaxisNode
    {
        public CollectionInitializerNode() : base() { }

        public override string NodeText() => "CollectionInitializerNode: {}";
    }

    public class ExpressionListNode : ExpressionNode
    {
        public ExpressionListNode() : base() { }

        public override string NodeText() => "ExpressionListNode: {}";
    }

    public class Statement_Expression_List : ExpressionNode
    {
        public Statement_Expression_List() : base() { }

        public override string NodeText() => "Statement_Expression_List is container and don't have token!";
    }
    #endregion

    public class ElementInitializerNode : SyntaxisNode
    {

        public ElementInitializerNode() : base() { }

        public override string NodeText() => "ElementInitializerNode:" + token.GetText();
    }

    public class GlobalNode : SyntaxisNode //содержит using, class и структуры enum/struct
    {
        public GlobalNode() : base() { }

        public override string NodeText() => "Do not have a value, because it's a Start(base) node!!!";
    }

    public class NamespaceBodyNode : SyntaxisNode
    {
        public NamespaceBodyNode() : base() { }

        public override string NodeText() => "NamespaceBodyNode: {}";
    }

    public class NamespaceDeclarationNode : SyntaxisNode
    {
        public NamespaceDeclarationNode() : base() { }

        public override string NodeText() => "NamespaceDeclarationNode:" + token.GetText();
    }


    public class UsingNode : SyntaxisNode
    {
        public UsingNode() : base() { }
        public override string NodeText() => "UsingNode";
    }

    public class QualifiedIdentifierNode : SyntaxisNode
    {
        public QualifiedIdentifierNode() : base() { }
        public override string NodeText() => "QualifiedIdentifierNode:" + token.GetText();
    }



    #region AtributesAndParameters
    /// <summary>
    /// Базовый класс для описания всевозможных атрибут,
    /// модификаторов и т.д., входящих в задекларированную форму записи
    /// класса, структуры и перечисления
    /// Type - базовый токен для [Type]+Node
    /// </summary>
    public class TypeComponentsNode : SyntaxisNode
    {
        public TypeComponentsNode() : base() { }
        public override string NodeText() => "Компонента типа:" + token.GetText();
    }

    public class TypeModificatorNode : TypeComponentsNode { public TypeModificatorNode() : base() { } }

    //возможно расширение и добавление атрибутов и т.д.
    #endregion

    #region Class
    public class ClassNode : SyntaxisNode
    {
        public ClassNode() : base() { }
        public override string NodeText() => "ClassNode:" + token.GetText();
    }

    public class ClassBodyNode : ClassNode
    {
        public ClassBodyNode() : base() { }
        public override string NodeText() => "ClassBody: {}";
    }
    #endregion

    #region Structure
    public class StructureNode : SyntaxisNode
    {
        public StructureNode() : base() { }
        public override string NodeText() => "StructureNode:" + token.GetText();
    }

    public class StructureBodyNode : StructureNode
    {
        public StructureBodyNode() : base() { }
        public override string NodeText() => "StructureBodyNode: {}";
    }
    #endregion

    #region Enum
    public class EnumNode:SyntaxisNode
    {
        public EnumNode() : base() { }
        public override string NodeText() => "EnumNode:" + token.GetText();
    }

    public class EnumBodyNode : EnumNode
    {
        public EnumBodyNode() : base() { }
        public override string NodeText() => "EnumBodyNode: {}";
    }
    #endregion
    public class ArrayInitializerNode : SyntaxisNode
    {
        public ArrayInitializerNode() : base() { }
        public override string NodeText() => "ArrayInitializerNode: {}";
    }


    #region MemberDeclaration
    public class DeclarationNode : SyntaxisNode
    {
        public DeclarationNode() : base() { }
        public override string NodeText() => "DeclarationNode:" + token.GetText();
    }
    public class ConstantDeclarationNode : DeclarationNode
    {
        public ConstantDeclarationNode() : base() { }
        public override string NodeText() => "ConstantDeclarationNode:" + token.GetText();
    }

    public class ConstantDeclaratorNode : DeclarationNode
    {
        public ConstantDeclaratorNode() : base() { }
        public override string NodeText() => "ConstantDeclaratorNode:" + token.GetText();
    }

    public class FieldDeclarationNode : DeclarationNode
    {
        public FieldDeclarationNode() : base() { }
        public override string NodeText() =>
            "Not have a any token because this node a container for any Field Declaration Nodes";
    }

    public class VariableDeclaratorNode : FieldDeclarationNode
    {
        public VariableDeclaratorNode() : base() { }
        public override string NodeText() => "VariableDeclaratorNode " + token.GetText();
    }

    public class ConstructorDeclarationNode : DeclarationNode
    {
        public ConstructorDeclarationNode() : base() { }
        public override string NodeText() => "Not have a any token because this node a container for any Constructor Declaration Nodes";
    }

    public class MethodDeclarationNode : DeclarationNode
    {
        public MethodDeclarationNode() : base() { }
        public override string NodeText() => "Not have a any token because this node na container for any Method Declaration Nodes";
    }

    public class DestructorDeclarationNode : DeclarationNode
    {
        public DestructorDeclarationNode() : base() { }
        public override string NodeText() => "DestructorDeclarationNode: " + token.GetText();
    }

    public class IdentifierDeclarationListNode : DeclarationNode
    {
        public IdentifierDeclarationListNode() : base() { }
        public override string NodeText() => "IdentifierDeclarationListNode is a container for IdentifierDeclarationNodes!";
    }
    public class IdentifierDeclarationNode : IdentifierDeclarationListNode
    {
        public IdentifierDeclarationNode() : base() { }
        public override string NodeText() => "IdentifierDeclarationNode is a container for identificatorNodes!";
    }

    public class IdentifierDeclarationWithConstantExpressionNode : IdentifierDeclarationListNode
    {
        public IdentifierDeclarationWithConstantExpressionNode() : base() { }
        public override string NodeText() => "IdentifierDeclarationWithConstantExpressionNode: " + token.GetText();
    }

    //etc
    #endregion

    #region ProgrammBlockNodes_And_Statements
    public class ProgrammBlockNode : SyntaxisNode
    {
        public ProgrammBlockNode() : base() { }

        public override string NodeText() => "ProgrammBlockNode: {}";
    }

    public class DeclarationStatementNode : ProgrammBlockNode
    {
        public DeclarationStatementNode() : base() { }

        public override string NodeText() => "Not have a any token because this node a container for any Method Declaration Nodes";
    }
    public class LocalVariableDeclaratorNode : DeclarationStatementNode
    {
        public LocalVariableDeclaratorNode() : base() { }

        public override string NodeText() => "LocalVariableDeclaratorNode " + token.ToString();
    }

    public class EmbededDeclaratorNode : DeclarationStatementNode
    {
        public EmbededDeclaratorNode() : base() { }

        public override string NodeText() => "Not have a any token because this node\r\na container for any Embeded Declarator Nodes";
    }
    
    public class EmptyStatementNode : EmbededDeclaratorNode
    {
        public EmptyStatementNode() : base() { }

        public override string NodeText() => "EmptyStatementNode " + token.ToString();
    }
    public class IfStatementNode : EmbededDeclaratorNode
    {
        public IfStatementNode() : base() { }

        public override string NodeText() => "IfStatementNode " + token.ToString();
    }

    public class SwitchStatementNode: EmbededDeclaratorNode
    {
        public SwitchStatementNode() : base() { }

        public override string NodeText() => "SwitchStatementNode " + token.ToString();
    }

    public class SwitchBlockNode : SwitchStatementNode
    {
        public SwitchBlockNode() : base() { }

        public override string NodeText() => "SwitchBlockNode: {}";
    }

    public class SwitchLlabelNode : SwitchStatementNode
    {
        public SwitchLlabelNode() : base() { }

        public override string NodeText() => "SwitchLlabelNode: " + token.GetText();
    }

    public class WhileStatementNode : EmbededDeclaratorNode
    {
        public WhileStatementNode() : base() { }

        public override string NodeText() => "WhileStatementNode: " + token.GetText();
    }
    

    public class DoStatementNode : EmbededDeclaratorNode
    {
        public DoStatementNode() : base() { }

        public override string NodeText() => "DoStatementNode: " + token.GetText();
    }

    public class ForStatementNode : EmbededDeclaratorNode
    {
        public ForStatementNode() : base() { }

        public override string NodeText() => "ForStatementNode: " + token.GetText();
    }

    public class BreakStatementNode : EmbededDeclaratorNode
    {
        public BreakStatementNode() : base() { }

        public override string NodeText() => "BreakStatementNode: " + token.GetText();
    }

    public class ContinueStatementNode : EmbededDeclaratorNode
    {
        public ContinueStatementNode() : base() { }

        public override string NodeText() => "ContinueStatementNode: " + token.GetText();
    }

    public class ReturnStatementNode : EmbededDeclaratorNode
    {
        public ReturnStatementNode() : base() { }

        public override string NodeText() => "ReturnStatementNode: " + token.GetText();
    }

    public class ExpressionStatementNode : EmbededDeclaratorNode
    {
        public ExpressionStatementNode() : base() { }

        public override string NodeText() => "this node is a container for StatementExpressionNode!";
    }


    //Все наследуемые statements
    #endregion

}
