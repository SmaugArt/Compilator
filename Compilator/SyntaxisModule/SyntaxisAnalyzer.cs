using Compilator.AnalyzerModule;
using Compilator.AnalyzerModule.AnalyzerStructures;
using Compilator.GeneralStructures;
using Compilator.SyntaxisModule.Structures;
using System;
using System.Collections.Generic;

namespace Compilator.SyntaxisModule
{
    public class SyntaxisAnalyzer
    {
        private Analyzer analyzer;

        #region PrintParameters
        private List<int> LengthOfStrLevel; //(максимальная) длина строки уровня (дерева)
        private List<string> StrLines; //список строк, которые нужны для отрисовки дерева 
        #endregion
        public SyntaxisAnalyzer(Analyzer analyzer)
        {
            this.analyzer = analyzer;
            LengthOfStrLevel = new List<int>();
            StrLines = new List<string>();
            int i = 3;
        }

        /// <summary>
        /// Метод проверяет обязательное условие, 
        /// что первый токен отличен от пустого
        /// таким образом левое значения всегда будет ненулевым!!!
        /// </summary>
        /// <returns></returns>
        public SyntaxisNode SyntaxisParse()
        {
            if (analyzer.GetToken() == null)
                return new NodeLiteral(){ token = new Token("", 0, 0, TokenType.Null)};

            analyzer.StepBack();
            return DoubleParsePlusMinus();
            int i = (int)10.1^(int)2.1;
        }

        private SyntaxisNode Compilation_Unit()
        {
            while (true)
            {
                var left = ParseFactor();
                if ()
                    UsingDirectives();
            }
            

        }



















        private SyntaxisNode DoubleParsePlusMinus() //expression c 2-мя уровнями приоритета +-
        {
            var left = DoubleParseMultiplyDivide(); //парсит терминал чекаем, что такое и при необходимости вызываем ошибку
            if (left.GetType() != typeof(NodeDouble) && left.GetType() != typeof(NodeInt) && left.GetType() != typeof(NodeBinaryOp))
                throw SynException.ShowException(EXType.IncorrectNode ,left.token.ToString());

            var t = analyzer.GetToken();

            if (t != null && (t.GetText() == "+" || t.GetText() == "-"))
                return new NodeBinaryOp() { token = t, children = new List<SyntaxisNode>() { left, DoubleParsePlusMinus()}};

            analyzer.StepBack();
            return left;

        }

        private SyntaxisNode DoubleParseMultiplyDivide() //term - слогаемое */
        {
            var left = ParseFactor(); //парсит терминал чекаем, что такое и при необходимости вызываем ошибку
            if (left.GetType() != typeof(NodeDouble) && left.GetType() != typeof(NodeInt))
                throw SynException.ShowException(EXType.IncorrectNode, left.token.ToString());

            var t = analyzer.GetToken();

            if (t != null && t.GetText() == "*" || t != null && t.GetText() == "/") //t!= null - проверка на конец считывания
                return new NodeBinaryOp() { token = t, children = new List<SyntaxisNode>() { left, DoubleParseMultiplyDivide() } };//LeftNode = left, RightNode = ParseTerm() };

            analyzer.StepBack();
            return left;
        }



        //private SyntaxisNode TypeCast() //приведение типов
        //{

        //}

        //private Declaration() { } //объявление переменной

        private SyntaxisNode ParsePrimaryExpression() //factor - Пуcть будет методом получения нода узла
        {
            Token t = analyzer.GetToken();

            if (t == null) return new EmptyNode();

            switch (t.GetTokenType())
            {
                //+++++Literal
                case TokenType.CharData:
                    return new NodeChar() { token = t };
                case TokenType.DoubleData:
                    return new NodeDouble() { token = t };
                case TokenType.IntData:
                    return new NodeInt() { token = t };
                case TokenType.StringData:
                    return new NodeString() { token = t };
                case (TokenType.KeyWord):
                    if((KeyWords.KW)t.value == KeyWords.KW.kwTrue|| (KeyWords.KW)t.value == KeyWords.KW.kwFalse)
                        return new NodeBool() { token = t };
                    if((KeyWords.KW)t.value == KeyWords.KW.kwNull)
                        return new NullNode() { token = t };
                    //++++object_creation_expression
                    if ((KeyWords.KW)t.value == KeyWords.KW.kwNew)
                    {
                        SyntaxisNode type = ParseType(); //получаю type (Некий идентификатор или KeyWord)
                        Token LeftCyrcleBr = analyzer.GetToken();
                        if (LeftCyrcleBr.GetTokenType() != TokenType.Operator || (Operators.OP)LeftCyrcleBr.value != Operators.OP.opLeftCurlyBracket)
                            throw new Exception("Ожидается \"(\", но получен:" + LeftCyrcleBr.ToString());
                        
                        while (true)
                        {
                            //параметры
                            List<SyntaxisNode> par = ParseObjectOrCollectionInitializer();//ParseArgumentList(); //char data == standart datatype
                        }

                        return new ObjectCreationExpressionNode() { token = t, children = new List<SyntaxisNode>() { DATA } };
                    }
                    //----object_creation_expression
                    break;
                //----Literal
                //++++Simple  name???????????????????????????????????
                case TokenType.Identificator:
                    return new NodeIdentificator() { token = t };
                //----Simple Nmae
                //++++parenthesized_expression
                case TokenType.Operator:
                    if ((Operators.OP)t.value == Operators.OP.opLeftCurlyBracket) //съедаем Expression
                    {
                        ExpressionNode node = ParseExpression();
                        Token t2 = analyzer.GetToken();
                        if (t2.GetTokenType() == TokenType.Operator && (Operators.OP)t2.value == Operators.OP.opRightCurlyBracket)
                            return node;
                        throw new Exception("Ожидается \")\", но получен:" + t2.ToString());
                    }
                    break;
                //----parenthesized_expression
                default:
                    throw SynException.ShowException(EXType.IncorrectToken, t.ToString());
            }

            while (true) //может быть [][] и т.д.
            {
                //++++member_access

                //----member_access
                //element_access
                //invocation expression
            }
        }









        private SyntaxisNode ParseFactor() //factor - Пуcть будет методом получения нода узла
        {
            Token t = analyzer.GetToken();

            if (t == null) return new EmptyNode();

            switch (t.GetTokenType())
            {
                case TokenType.Identificator:
                    return new NodeIdentificator() { token = t };
                case TokenType.KeyWord:
                    return new NodeIdentificator() { token = t };
                //?case TokenType.Operator: ?-Unary or Binary operations
                case TokenType.CharData:
                    return new NodeChar() { token = t };
                case TokenType.DoubleData:
                    return new NodeDouble() { token = t };
                case TokenType.IntData:
                    return new NodeInt() { token = t };
                case TokenType.StringData:
                    return new NodeString() { token = t };
                default:
                    throw SynException.ShowException(EXType.IncorrectToken,t.ToString());
            }
        }


        #region Print

        //public string PrintTree(SyntaxisNode node)
        //{
        //    int treeLength = GetTreeLength(node, 1);
        //    for (int i = 0; i < treeLength; i++) LengthOfStrLevel.Add(0);

        //    SetMaxLevelStrLength(node, 0);
        //    for (int i = 0; i < SummLength(); i++) StrLines.Add("");

        //    PrintTreeLines(node, 0);
        //    string OutStr="";
        //    foreach (string item in StrLines) OutStr += item + "\r\n";

        //    StrLines.Clear();
        //    LengthOfStrLevel.Clear();
        //    return OutStr;
        //}

        //private int GetTreeLength(SyntaxisNode node, int startPos)
        //{
        //    int length = startPos;

        //    if (node == null) return 0; //пустое дерево имеет 0-ю длину

        //    //предполагается, что вдереве не содержится некорректных узлов
        //    if (node.GetType() == typeof(NodeBinaryOp))
        //    {
        //        var newNode = node as NodeBinaryOp;
        //        //length = newNode.token.GetText().Length;

        //        int leftLength = GetTreeLength(newNode.LeftNode, startPos+1);
        //        length = (leftLength > length) ? leftLength :length;

        //        int rightLength = GetTreeLength(newNode.RightNode, startPos + 1);
        //        length = (rightLength > length) ? rightLength : length;

        //        return length;
        //    }

        //    if (node.GetType() == typeof(NodeUnaryOp))
        //    {
        //        var newNode = node as NodeUnaryOp;

        //        int argLength = GetTreeLength(newNode.arg, startPos + 1);
        //        length = (argLength > length) ? argLength : length;
        //    }

        //    return length;
        //}

        ///// <summary>
        ///// устанавливает максимальное значение длины строки на каждом уровне дерева
        ///// в уже проинициализированный глобальный список
        ///// </summary>
        ///// <param name="node">Корневой узел дерева</param>
        ///// <param name="startPos">Стартовый уровень</param>
        //private void SetMaxLevelStrLength(SyntaxisNode node, int startPos)
        //{
        //    if (node == null) return;

        //    int length = node.token.GetText().Length;

        //    if (node.GetType() == typeof(NodeBinaryOp))
        //    {
        //        var newToken = node as NodeBinaryOp;
        //        SetMaxLevelStrLength(newToken.LeftNode, startPos + 1);
        //        SetMaxLevelStrLength(newToken.RightNode, startPos + 1);
        //    }

        //    if (node.GetType() == typeof(NodeUnaryOp))
        //    {
        //        var newToken = node as NodeUnaryOp;
        //        SetMaxLevelStrLength(newToken.arg, startPos + 1);
        //    }

        //    if (LengthOfStrLevel[startPos] < length) LengthOfStrLevel[startPos] = length;

        //    return;
        //}

        ////суммирует общее число символов, требующихся для построения дерева
        //private int SummLength()
        //{
        //    int length = 0;

        //    foreach (int item in LengthOfStrLevel)
        //        length += item;

        //    return length;
        //}

        ///// <summary>
        ///// Возвращает начальную позицию в массиве строк
        ///// </summary>
        ///// <param name="level">от 1 до N</param>
        ///// <returns></returns>
        //private int GetStrLevelPos(int level)
        //{
        //    if (level == null || level <= 0) throw new Exception("Level value can not be less a one!\nЗначение уровня не может быть меньше чем 1!");

        //    int pos = 0;
        //    for (int i = 0; i < level-1; i++)
        //        pos += LengthOfStrLevel[i];

        //    return pos;
        //}
        
        ///// <summary>
        ///// Подсчитывает количество отступов, требуемых для построения узла дерева
        ///// </summary>
        ///// <param name="maxLevel">от 1 до N</param>
        ///// <param name="currentLevel">от 1 до N</param>
        ///// <returns></returns>
        //private int GetLevelOffset(int maxLevel, int currentLevel)
        //{
        //    if (maxLevel <= 0 || currentLevel <= 0 || currentLevel > maxLevel) throw new Exception("Неправильно задан диапазон значений");

        //    int iterationValue = maxLevel - currentLevel;
        //    int value = 0;

        //    for (int i = 0; i < iterationValue; i++) value = value * 2 + 1;

        //    return value;
        //}

        //private void SetSimilarSymbolsFromEndStr(ref string str, char symbol, int value)
        //{
        //    for (int i = 0; i < value; i++) str += symbol;
        //}

        //private void PrintTreeLines(SyntaxisNode node, int startPos)
        //{
        //    int offset = GetLevelOffset(LengthOfStrLevel.Count, startPos + 1); //+1 т.к. отсчитываем от 0-ля
        //    int startStrPosition = GetStrLevelPos(startPos+1);//????????????????????????????????????????????????????????????????
        //    //normalize? или 1 раз спуститься вниз c вставкой от 1 до N количества пробелов?

        //    string str = node.token.GetText();

        //    for (int i = 0; i < LengthOfStrLevel[startPos]; i++)//str.Length; i++)
        //    {
        //        string strOfList = StrLines[startStrPosition + i];
        //        SetSimilarSymbolsFromEndStr(ref strOfList, '▓', offset);///////' ', offset);//до символа /

        //        strOfList += (i + 1 == LengthOfStrLevel[startPos] && node.GetType() == typeof(NodeUnaryOp) ||
        //            i + 1 == LengthOfStrLevel[startPos] && node.GetType() == typeof(NodeBinaryOp)) ? "/" : "▓";//" ";

        //        if(i+1 != LengthOfStrLevel[startPos])
        //            SetSimilarSymbolsFromEndStr(ref strOfList, '▓', offset);//' ', offset);//до символа текста
        //        else
        //            SetSimilarSymbolsFromEndStr(ref strOfList, ' ', offset);

        //        strOfList += (i + 1 <= str.Length) ? str.Substring(i, 1) : " "; //символ текста или " " вместо него

        //        if (i + 1 != LengthOfStrLevel[startPos])
        //            SetSimilarSymbolsFromEndStr(ref strOfList, '▓', offset); //' ', offset);//до символа "\"
        //        else
        //            SetSimilarSymbolsFromEndStr(ref strOfList, ' ', offset);

        //        strOfList += (LengthOfStrLevel[startPos] == i + 1 && node.GetType() == typeof(NodeBinaryOp)) ? "\\" : "▓";//" ";
        //        SetSimilarSymbolsFromEndStr(ref strOfList, '▓', offset);//////' ', offset);//до конца

        //        StrLines[startStrPosition + i] = strOfList;
        //    }

        //    if (node.GetType() == typeof(NodeUnaryOp))
        //    {
        //        var newNode = node as NodeUnaryOp;
        //        PrintTreeLines(newNode.arg, startPos + 1);
        //        NormalizeStrList(StrLines[startStrPosition].Length, GetStrLevelPos(startPos + 2)); //сразу все заполнит
        //    }

        //    if (node.GetType() == typeof(NodeBinaryOp))
        //    {
        //        var newNode = node as NodeBinaryOp;
        //        PrintTreeLines(newNode.LeftNode, startPos + 1);
        //        NormalizeStrList(StrLines[startStrPosition].Length - 2 * offset - 1, GetStrLevelPos(startPos + 2));
        //        PrintTreeLines(newNode.RightNode, startPos + 1);
        //        NormalizeStrList(StrLines[startStrPosition].Length, GetStrLevelPos(startPos + 2));
        //    }
        //}

        //private void NormalizeStrList(int maxStrLength, int startPos)
        //{
        //    for (int i = startPos; i < StrLines.Count; i++)
        //    {
        //        int insertValue = maxStrLength - StrLines[i].Length;

        //        for (int i2 = 0; i2 < insertValue; i2++)
        //           StrLines[i] += "▓";//" ";
        //    }
        //}
        #endregion
    }
}
