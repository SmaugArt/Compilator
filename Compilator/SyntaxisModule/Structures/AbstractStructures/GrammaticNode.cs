using Compilator.GeneralStructures;

namespace Compilator.SyntaxisModule.Structures.AbstractStructures
{
    public enum SN
    {
        SyntaxNode,
        LiteralNode,
        BinaryOperationNode,
        UnaryOperationNode,
        IdentificatorNode
    };

    public enum Term
    {
        Empty,
        Terminal,
        NotATerminal //хранит ссылку на следующую грамматику (более низкого уровня)
    }

    public abstract class GrammaticBody
    {
        public Term typeOfGrammaticBody;

        public bool TypeEquals(string type) => typeOfGrammaticBody.ToString().Equals(type)?  true : false;
    }

    public class NotATerminal: GrammaticBody
    {
        public AbstractGrammatic grammatic;
    }
    public class Terminal: GrammaticBody
    {
        public SN SyntaxNodeType;
        public TokenType tokenType;
        public string bodyGrammatic; //for keyword and operators (ключевые зарезервированные слова)

        /// <summary>
        /// Создает экземпляр класса Terminal.
        /// </summary>
        /// <param name="SyntaxNodeType">Тип узла синтаксического дерева разбора</param>
        /// <param name="tokenType">Тип токена</param>
        /// <param name="bodyGrammatic">Строка соответствия строки токена. При Null не учитывается в проверке</param>
        public Terminal(SN SyntaxNodeType, TokenType tokenType, string bodyGrammatic = null)
        {
            this.SyntaxNodeType = SyntaxNodeType;
            this.tokenType = tokenType;
            this.bodyGrammatic = bodyGrammatic;
        }

        public bool isValid(SyntaxisNode node)
        {
            if (node.typeNode != SyntaxNodeType.ToString()) return false;

            if (node.token.GetTokenType() != tokenType) return false;

            if (bodyGrammatic != null && bodyGrammatic != node.token.GetText()) return false;

            return true;
        }
    }

    public class EmptyTerminal : GrammaticBody { public EmptyTerminal() => typeOfGrammaticBody = Term.Empty; }
}
