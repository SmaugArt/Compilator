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
				return new NodeLiteral() { token = new Token("", 0, 0, TokenType.Null) };

			analyzer.StepBack();
			return DoubleParsePlusMinus();
			int i = (int)10.1 ^ (int)2.1;
		}

		private SyntaxisNode Compilation_Unit()
		{
			while (true) {
				var left = ParseFactor();
				if ()
					UsingDirectives();
			}


		}



		private SyntaxisNode ParsePimaryExpression() //without array_creation_expression
		{
			SyntaxisNode parsePNACE = Parse_Primary_No_Array_Creation_Expression();

			Token token = analyzer.GetToken();
			if (token == null) return parsePNACE;

			//+++++member_access
			if (token.GetTokenType() == TokenType.Operator && (Operators.OP)token.value == Operators.OP.opDot) {
				Token token2 = analyzer.GetToken();

				if (token2 == null || token2.GetTokenType() != TokenType.Identificator) //without type_argument_list
					throw SynException.ShowException(EXType.IncorrectToken, "");

				return new MemberAccessNode() { token = token, children = new List<SyntaxisNode>()
								 { parsePNACE, new NodeIdentificator() { token = token2 } }
				};
			}
			//-----member_access

			//+++++post increment expression
			if (token.GetTokenType() == TokenType.Operator && (Operators.OP)token.value == Operators.OP.opIncrementExpression)
				return new PostIncrementNode() { token = token, children = new List<SyntaxisNode>() { parsePNACE } };
			//-----post increment expression

			//+++++post decrement expression
			if (token.GetTokenType() == TokenType.Operator && (Operators.OP)token.value == Operators.OP.opDecrementExpression)
				return new PostDecrementNode() { token = token, children = new List<SyntaxisNode>() { parsePNACE } };
			//-----post decrement expression

			//+++++element_access
			if (token.GetTokenType() == TokenType.Operator && (Operators.OP)token.value == Operators.OP.opLeftSquareBracket) {
				ExpressionNode newNode = ParseExpression();

				Token token2 = analyzer.GetToken();
				if (token2 == null || token2.GetTokenType() != TokenType.Operator || (Operators.OP)token2.value != Operators.OP.opRightSquareBracket)
					throw new Exception("Ожидается \"]\", но получен:" + ((token2 == null) ? "Null" : token2.ToString()));

				return new ElementAccessNode() { token = token, children = new List<SyntaxisNode>() { parsePNACE, newNode } };
			}
			//-----element_access

			//+++++invocation expression
			if (token.GetTokenType() == TokenType.Operator && (Operators.OP)token.value == Operators.OP.opLeftCurlyBracket) //without parameters constructor
			{
				Token token2 = analyzer.GetToken();
				if (token2 == null || token2.GetTokenType() != TokenType.Operator || (Operators.OP)token2.value != Operators.OP.opRightCurlyBracket)
					throw new Exception("Ожидается \")\", но получен:" + ((token2 == null) ? "Null" : token2.ToString()));

				return new InvocationExpressionNode() { token = token, children = new List<SyntaxisNode>() { parsePNACE } };
			}
			//-----invocation expression

			analyzer.StepBack();
			return parsePNACE;
		}

		private ExpressionNode Parse_Primary_No_Array_Creation_Expression() //like a Primary expression
		{
			Token t = analyzer.GetToken();

			if (t == null) throw SynException.ShowException(EXType.NullNode, t.ToString());//return new EmptyNode(); //?

			switch (t.GetTokenType()) {
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
					if ((KeyWords.KW)t.value == KeyWords.KW.kwTrue || (KeyWords.KW)t.value == KeyWords.KW.kwFalse)
						return new NodeBool() { token = t };
					if ((KeyWords.KW)t.value == KeyWords.KW.kwNull)
						return new NullNode() { token = t };
					//++++object_creation_expression ///Делаем без переменных, иначе придется реализовывать деревоы
					if ((KeyWords.KW)t.value == KeyWords.KW.kwNew) {
						SyntaxisNode type = ParseType(); //получаю type (Некий идентификатор или KeyWord)

						Token opLeftParenthesis = analyzer.GetToken();
						if (opLeftParenthesis.GetTokenType() != TokenType.Operator || (Operators.OP)opLeftParenthesis.value != Operators.OP.opLeftParenthesis)
							throw new Exception("Expected \"(\", but get:" + opLeftParenthesis.ToString());
						Token opRightParenthesis = analyzer.GetToken();

						if (opRightParenthesis.GetTokenType() != TokenType.Operator || (Operators.OP)opRightParenthesis.value != Operators.OP.opRightParenthesis)
							throw new Exception("Expected \")\", but get:" + opRightParenthesis.ToString());

						Token LeftCyrkleBrace = analyzer.GetToken();
						if (LeftCyrkleBrace.GetTokenType() != TokenType.Operator || (Operators.OP)LeftCyrkleBrace.value != Operators.OP.opLeftCurlyBracket)
							throw new Exception("Expected \"{\", but get:" + LeftCyrkleBrace.ToString());

						//параметры
						List<SyntaxisNode> par = ParseObjectOrCollectionInitializer();//ParseArgumentList(); //char data == standart datatype

						Token RightCyrkleBrace = analyzer.GetToken();
						if (RightCyrkleBrace.GetTokenType() != TokenType.Operator || (Operators.OP)RightCyrkleBrace.value != Operators.OP.opRightCurlyBracket)
							throw new Exception("Expected \"{\", but get:" + RightCyrkleBrace.ToString());

						return new ObjectCreationExpressionNode() { token = t, children = par };
					}
					//----object_creation_expression
					break;
				//----Literal

				//++++Simple  name???????????????????????????????????
				case TokenType.Identificator:
					return new NodeIdentificator() { token = t };
				//----Simple Name

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
			}

			throw SynException.ShowException(EXType.IncorrectToken, t.ToString());
		}

		private ExpressionNode ParseExpression() //without lambda and query and assigmentExpression
		{
			return Parse_Conditional_Expression();
		}

		private ExpressionNode Parse_Conditional_Expression()
		{
			var left = Null_Coalescing_Expression();

			Token token1 = analyzer.GetToken();

            if (token1 != null && token1.GetTokenType() == TokenType.Operator && token1.value.Equals(Operators.OP.opQuestionMark))
            {
                var middle = ParseExpression();
                Token tokenDoubleDot = analyzer.GetToken();

                if (tokenDoubleDot == null || tokenDoubleDot.GetTokenType() != TokenType.Operator || !tokenDoubleDot.value.Equals(Operators.OP.opDoubleDot))
                    throw new Exception("Expected a \":\", bu get:" + tokenDoubleDot?.GetText());

                var right = ParseExpression();

                return new ConditionalExpressionNode() { token = token1, children = new List<SyntaxisNode>() {left, middle, right } };
			} 

            analyzer.StepBack();
            return left;
		}

		private ExpressionNode Null_Coalescing_Expression() => Conditional_Or_Expression();

        private ExpressionNode Conditional_Or_Expression() //||
        {
            var left = Conditional_And_Expression();

            Token operatorOr = analyzer.GetToken(); //||

            if (operatorOr != null && operatorOr.GetTokenType() == TokenType.Operator && operatorOr.value.Equals(Operators.OP.opOr))
            {
                return new ConditionalOrExpressionNode() { token = operatorOr, children = new List<SyntaxisNode>() { left, Conditional_Or_Expression() } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode Conditional_And_Expression() // &&
        {
            var left = Inclusive_Or_Expression();
            Token operatorAnd = analyzer.GetToken();

            if(operatorAnd != null && operatorAnd.GetTokenType() == TokenType.Operator && operatorAnd.value.Equals(Operators.OP.opAnd))
            {
                return new ConditionalAndExpressionNode() { token = operatorAnd, children = new List<SyntaxisNode>() { left, Conditional_And_Expression() } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode Inclusive_Or_Expression() //|
        {
            var left = Exclusive_Or_Expression();
            Token opLogicalOr = analyzer.GetToken();

            if (opLogicalOr != null && opLogicalOr.GetTokenType() == TokenType.Operator && opLogicalOr.value.Equals(Operators.OP.opLogicalOr))
            {
                return new InclusiveOrExpressionNode() { token = opLogicalOr, children = new List<SyntaxisNode>() { left, Inclusive_Or_Expression() } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode Exclusive_Or_Expression() //^ = функциональое или (0^0=0,0^1=1,1^0,1^1=0)
        {
            var left = And_Expression();
            Token opExclusiveOr = analyzer.GetToken();

            if (opExclusiveOr != null && opExclusiveOr.GetTokenType() == TokenType.Operator && opExclusiveOr.value.Equals(Operators.OP.opToDegree))
            {
                return new ExclusiveOrExpressionNode() { token = opExclusiveOr, children = new List<SyntaxisNode>() { left, Exclusive_Or_Expression() } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode And_Expression()
        {
            var left = EqualityExpression();
            Token opLogicalAnd = analyzer.GetToken();

            if (opLogicalAnd != null && opLogicalAnd.GetTokenType() == TokenType.Operator && opLogicalAnd.value.Equals(Operators.OP.opLogicalAnd))
            {
                return new AndExpressionNode() { token = opLogicalAnd, children = new List<SyntaxisNode>() { left, And_Expression() } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode EqualityExpression() // ==, !=
        {
            var left = Relational_Expression_IS_AS();
            Token opEqualOrNotEqual = analyzer.GetToken();

            if (opEqualOrNotEqual != null && opEqualOrNotEqual.GetTokenType() == TokenType.Operator && 
                (opEqualOrNotEqual.value.Equals(Operators.OP.opDoubleEquals) || opEqualOrNotEqual.value.Equals(Operators.OP.opNotEquals)))
            {
                return new EqualityExpressionNode() { token = opEqualOrNotEqual, children = new List<SyntaxisNode>() { left, EqualityExpression() } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode Relational_Expression_IS_AS() //избавление от лево-рекурсивного зацикливания в Relational_Expression
        {
            var left = Relational_Expression();
            var kwISorAS = analyzer.GetToken();

            if (kwISorAS != null && kwISorAS.GetTokenType() == TokenType.KeyWord &&
                (kwISorAS.value.Equals(KeyWords.KW.kwIs) || kwISorAS.value.Equals(KeyWords.KW.kwAs)))
            {
                return new RelationalExpressionNode() { token = kwISorAS, children = new List<SyntaxisNode>() { left, TypeParse() } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode Relational_Expression()
        {
            var left = ShiftExpression();
            Token token = analyzer.GetToken();

            if (token == null || token.GetTokenType() != TokenType.Operator)
                throw SynException.ShowException(EXType.IncorrectToken, (token == null)? "Null reference":token.ToString());

            switch ((Operators.OP)token.value)
            {
                case Operators.OP.opGreater: //>                 
                case Operators.OP.opLess: //<
                case Operators.OP.opLessOrEquals: //>=
                case Operators.OP.opGreaterOrEquals: //<=
                    return new RelationalExpressionNode()
                    {
                        token = token,
                        children = new List<SyntaxisNode>()
                        {
                            left,
                            Relational_Expression_IS_AS()
                        }
                    };
            }

            analyzer.StepBack();
            return left;
        }

        /// <summary>
        /// without right_shift additive(>>) and left_shift additive(<<)
        /// </summary>
        /// <returns></returns>
        private ExpressionNode ShiftExpression() => Additive_Expression();

        private ExpressionNode Additive_Expression() //+-
        {
            var left = Multiplicative_Expression();
            Token token = analyzer.GetToken();

            if (token != null && token.GetTokenType() == TokenType.Operator &&
                (token.value.Equals(Operators.OP.opPlus) || token.value.Equals(Operators.OP.opMinus)))
            {
                var right = Additive_Expression();
                return new BinaryOperationExpressionNode() { token=token, children=new List<SyntaxisNode>() {left, right } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode Multiplicative_Expression() //*,/ without a %
        {
            var left = Unary_Expression_Primary_Part();
            Token token = analyzer.GetToken();

            if (token != null && token.GetTokenType() == TokenType.Operator &&
                (token.value.Equals(Operators.OP.opAsterisk) || token.value.Equals(Operators.OP.opRightSlash)))
            {
                var right = Multiplicative_Expression();
                return new BinaryOperationExpressionNode() { token = token, children = new List<SyntaxisNode>() { left, right } };
            }

            analyzer.StepBack();
            return left;
        }

        /// <summary>
        /// Избавление от многократной отмены действий, вызванных
        /// переходом к Primary_Expression
        /// </summary>
        /// <returns></returns>
        private ExpressionNode Unary_Expression_Primary_Part() 
        {
            var left = Unary_Expression();

            if (left == null) return Parse_Primary_No_Array_Creation_Expression();

            return left;
        }

        private ExpressionNode Unary_Expression()
        {
            Token token = analyzer.GetToken();

            switch ((Operators.OP)token.value)
            {
                case Operators.OP.opPlus:
                case Operators.OP.opMinus:
                case Operators.OP.opIncrementExpression:
                case Operators.OP.opDecrementExpression:
                case Operators.OP.opExclamation:
                    return new UnaryOperationExpressionNode()
                    {
                        token = token,
                        children = new List<SyntaxisNode>()
                        {
                            Unary_Expression_Primary_Part()
                        }
                    };
                case Operators.OP.opLeftCurlyBracket:
                    var type = TypeParse();
                    Token leftCyrklyBrasket = analyzer.GetToken();

                    if (leftCyrklyBrasket == null || leftCyrklyBrasket.GetTokenType() != TokenType.Operator ||
                        !leftCyrklyBrasket.value.Equals(Operators.OP.opRightCurlyBracket))
                        throw SynException.ShowException(EXType.IncorrectToken, (leftCyrklyBrasket ==null)?"Expected \")\"": leftCyrklyBrasket.ToString());
                    return new CastEspression() { token= token, children = new List<SyntaxisNode>() { type, Unary_Expression_Primary_Part() } }; 

            }

            return null;
        }


        //private SyntaxisNode ParseFactor() //factor - Пуcть будет методом получения нода узла
        //{
        //    Token t = analyzer.GetToken();

        //    if (t == null) return new EmptyNode();

        //    switch (t.GetTokenType())
        //    {
        //        case TokenType.Identificator:
        //            return new NodeIdentificator() { token = t };
        //        case TokenType.KeyWord:
        //            return new NodeIdentificator() { token = t };
        //        //?case TokenType.Operator: ?-Unary or Binary operations
        //        case TokenType.CharData:
        //            return new NodeChar() { token = t };
        //        case TokenType.DoubleData:
        //            return new NodeDouble() { token = t };
        //        case TokenType.IntData:
        //            return new NodeInt() { token = t };
        //        case TokenType.StringData:
        //            return new NodeString() { token = t };
        //        default:
        //            throw SynException.ShowException(EXType.IncorrectToken,t.ToString());
        //    }
        //}


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
