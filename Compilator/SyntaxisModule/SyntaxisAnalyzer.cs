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


        #region Primary_Expression
        private SyntaxisNode ParsePimaryExpression() //without array_creation_expression
        {
            SyntaxisNode parsePNACE = Parse_Primary_No_Array_Creation_Expression();

            Token token = analyzer.GetToken();
            if (token == null) return parsePNACE;

            //+++++member_access
            if (token.GetTokenType() == TokenType.Operator && (Operators.OP)token.value == Operators.OP.opDot) {

                return new MemberAccessNode() { token = token, children = new List<SyntaxisNode>()
                                 { parsePNACE, Parse_Identificator() }
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
            if (token.GetTokenType() == TokenType.Operator && (Operators.OP)token.value == Operators.OP.opLeftParenthesis) //without parameters constructor
            {
                Token token2 = analyzer.GetToken();
                if (token2 == null || token2.GetTokenType() != TokenType.Operator || (Operators.OP)token2.value != Operators.OP.opRightParenthesis)
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
                        SyntaxisNode type = Type_Parse_Level1(); //получаю type (Некий идентификатор или KeyWord)

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
                        SyntaxisNode par = ParseObjectOrCollectionInitializer();//ParseArgumentList(); //char data == standart datatype

                        Token RightCyrkleBrace = analyzer.GetToken();
                        if (RightCyrkleBrace.GetTokenType() != TokenType.Operator || (Operators.OP)RightCyrkleBrace.value != Operators.OP.opRightCurlyBracket)
                            throw new Exception("Expected \"{\", but get:" + RightCyrkleBrace.ToString());

                        return new ObjectCreationExpressionNode() { token = t, children = new List<SyntaxisNode>() { par } };
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
                    if ((Operators.OP)t.value == Operators.OP.opLeftParenthesis) //съедаем Expression
                    {
                        ExpressionNode node = ParseExpression();
                        Token t2 = analyzer.GetToken();
                        if (t2.GetTokenType() == TokenType.Operator && (Operators.OP)t2.value == Operators.OP.opRightParenthesis)
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
            int stepBackCount = analyzer.stepBackCount;

            try
            {
                return Parse_Conditional_Expression();
            }
            catch
            {
                for (int i = analyzer.stepBackCount; i > stepBackCount;)
                    analyzer.StepBack();

                return Assignment_Parse();
            }

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

                return new ConditionalExpressionNode() { token = token1, children = new List<SyntaxisNode>() { left, middle, right } };
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

            if (operatorAnd != null && operatorAnd.GetTokenType() == TokenType.Operator && operatorAnd.value.Equals(Operators.OP.opAnd))
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
                return new RelationalExpressionNode() { token = kwISorAS, children = new List<SyntaxisNode>() { left, Type_Parse_Level1() } };
            }

            analyzer.StepBack();
            return left;
        }

        private ExpressionNode Relational_Expression()
        {
            var left = ShiftExpression();
            Token token = analyzer.GetToken();

            if (token == null || token.GetTokenType() != TokenType.Operator)
                throw SynException.ShowException(EXType.IncorrectToken, (token == null) ? "Null reference" : token.ToString());

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
                return new BinaryOperationExpressionNode() { token = token, children = new List<SyntaxisNode>() { left, right } };
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
                case Operators.OP.opLeftParenthesis:
                    var type = Type_Parse_Level1();
                    Token leftCyrklyBrasket = analyzer.GetToken();

                    if (leftCyrklyBrasket == null || leftCyrklyBrasket.GetTokenType() != TokenType.Operator ||
                        !leftCyrklyBrasket.value.Equals(Operators.OP.opRightParenthesis))
                        throw SynException.ShowException(EXType.IncorrectToken, (leftCyrklyBrasket == null) ? "Expected \")\"" : leftCyrklyBrasket.ToString());
                    return new CastEspression() { token = token, children = new List<SyntaxisNode>() { type, Unary_Expression_Primary_Part() } };

            }

            return null;
        }

        private SyntaxisNode Type_Parse_Level1() //убираем указатели type_unsafe
        {
            var Node = Type_Parse_Level2();
            Token token = analyzer.GetToken();
            //условия
            if (token == null)
            {
                analyzer.StepBack();
                return Node;
            }

            //+++++nullable_type
            if (token.GetTokenType() == TokenType.Operator && token.value.Equals(Operators.OP.opQuestionMark))
                return new NullableTypeNode() { token = token, children = new List<SyntaxisNode>() { Node } };
            //-----nullable_type

            //+++++Array_type
            //реализуем многомерные массивы !?
            if (token.GetTokenType() == TokenType.Operator && token.value.Equals(Operators.OP.opLeftSquareBracket))
            {
                List<SyntaxisNode> node = new List<SyntaxisNode>();
                Token coma = analyzer.GetToken();

                while (coma != null && coma.GetTokenType() == TokenType.Operator
                    && coma.value.Equals(Operators.OP.opComma))
                {
                    node.Add(new OperatorNode() { token = coma });
                    coma = analyzer.GetToken();
                }
                //analyzer.StepBack();

                Token rightSquareBR = coma;//analyzer.GetToken();
                if (rightSquareBR == null || rightSquareBR.GetTokenType() != TokenType.Operator
                    || !rightSquareBR.value.Equals(Operators.OP.opRightSquareBracket))
                    throw SynException.ShowException(EXType.IncorrectToken, (rightSquareBR == null) ?
                        "Null reference exeption" : "Expected \"]\", but get " + rightSquareBR.GetText());

                return new ArrayTypeNode() { token = token, children = node };//
            }
            //-----Array_type

            analyzer.StepBack();
            return Node;
        }

        private SyntaxisNode Type_Parse_Level2() //убираем указатели type_unsafe
        {
            Token token = analyzer.GetToken();

            if (token == null || token.GetTokenType() != TokenType.KeyWord && token.GetTokenType() != TokenType.Identificator)
                throw SynException.ShowException(EXType.IncorrectToken,
                    (token == null) ? "Null reference Exception" : "TokenType = " + token.GetTokenType());

            //в случае KyeWord
            if (token.GetTokenType() != TokenType.KeyWord)
                switch ((KeyWords.KW)token.value)
                {
                    case KeyWords.KW.kwBool: //реализую здесь
                    case KeyWords.KW.kwInt:
                    case KeyWords.KW.kwDouble:
                    case KeyWords.KW.kwString:
                    case KeyWords.KW.kwChar:
                        return new SimpleTypeNode() { token = token };
                }

            //+++++type_parameter
            ///return Parse_Identificator(token);  //аналог Parse_Identificator
            //type_parameter совмещаю с type_name, так как в обоих случаях используют identificator
            return Type_Name(token);
        }

        private NodeIdentificator Parse_Identificator(Token tokenRef = null)
        {
            Token token = (tokenRef == null) ? analyzer.GetToken() : tokenRef;

            if (token == null || token.GetTokenType() != TokenType.Identificator)
                throw SynException.ShowException(EXType.IncorrectToken,
                    (token == null) ? "Null reference Exception" : "TokenType = " + token.GetTokenType());
            return new NodeIdentificator() { token = token };
        }

        private SyntaxisNode Type_Name(Token tokenRef = null) => Namespace_Name(tokenRef);


        private SyntaxisNode Namespace_Name(Token tokenRef = null) => Namespace_Or_Type_Name(tokenRef);

        /// <summary>
        /// Without type_argument_list
        /// </summary>
        /// <param name="tokenRef"></param>
        /// <returns></returns>
        private SyntaxisNode Namespace_Or_Type_Name(Token tokenRef = null)
        {
            Token token = (tokenRef == null) ? analyzer.GetToken() : tokenRef;

            if (token == null || token.GetTokenType() != TokenType.Identificator)
                throw SynException.ShowException(EXType.IncorrectToken,
                    (token == null) ? "Null reference Exception" : "TokenType = " + token.GetTokenType());
            Token dot = analyzer.GetToken();

            if (dot != null && dot.GetTokenType() == TokenType.Operator && dot.value.Equals(Operators.OP.opDot))
                return new OperatorNode() { token = dot, children = new List<SyntaxisNode> { new NodeIdentificator() { token = token }, Namespace_Or_Type_Name() } };

            analyzer.StepBack();
            return new NodeIdentificator() { token = token };
        }

        /// <summary>
        /// without: %= ,&= ,|= ,^= ,<<= ,right_shift_assignment
        /// </summary>
        /// <returns></returns>
        private ExpressionNode Assignment_Parse()
        {
            ExpressionNode node = Unary_Expression_Primary_Part();
            Token assigmentToken = analyzer.GetToken();

            if (assigmentToken == null || assigmentToken.GetTokenType() != TokenType.Operator)
                throw SynException.ShowException(EXType.IncorrectToken);

            switch ((Operators.OP)assigmentToken.value)
            {
                case Operators.OP.opEquals:
                    return new AssignmentNode()
                    {
                        token = assigmentToken,
                        children = new List<SyntaxisNode>
                                        {
                                            node,
                                            ParseExpression()
                                         }
                    };
                case Operators.OP.opPlus:
                case Operators.OP.opMinus:
                case Operators.OP.opAsterisk:
                case Operators.OP.opRightSlash:
                    Token equals = analyzer.GetToken();
                    if (equals == null || equals.GetTokenType() != TokenType.Operator && equals.value.Equals(Operators.OP.opEquals))
                        throw SynException.ShowException(EXType.IncorrectToken);
                    return new AssignmentNode()
                    {
                        token = assigmentToken,
                        children = new List<SyntaxisNode>
                                        {
                                            new OperatorNode(){ token=equals},
                                            node,
                                            ParseExpression()
                                         }
                    };
                default: throw SynException.ShowException(EXType.IncorrectToken);
            }
        }

        private SyntaxisNode ParseObjectOrCollectionInitializer()
        {
            int lastStepBackCount = analyzer.stepBackCount;

            try
            {
                return Object_Initializer();
            }
            catch
            {
                for (int i = analyzer.stepBackCount; i > lastStepBackCount)
                    analyzer.StepBack();
                return Collection_Initializer();
            }
        }

        private SyntaxisNode Object_Initializer()
        {
            Token leftFiqureBR = analyzer.GetToken();
            if (leftFiqureBR == null || leftFiqureBR.GetTokenType() != TokenType.Operator ||
                !leftFiqureBR.value.Equals(Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected \"{\"");

            //can be a null!!!
            List<SyntaxisNode> node = Member_Initializer_List();
            Token rightFiqureBR = analyzer.GetToken();
            if (rightFiqureBR == null || rightFiqureBR.GetTokenType() != TokenType.Operator ||
                !rightFiqureBR.value.Equals(Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected \"}\"");

            if (node == null || node.Count==0)
                return new ObjectInitializerNode() { token = leftFiqureBR };

            return new ObjectInitializerNode() { token = leftFiqureBR, children = node };
        }

        /// <summary>
        /// совокупность MemberInitializerNode и OperatorsNode
        /// </summary>
        /// <returns></returns>
        private List<SyntaxisNode> Member_Initializer_List()
        {
            List<SyntaxisNode> listOfNode = new List<SyntaxisNode>();

            while (true) //псевдо-бесконечный цикл
            {
                MemberInitializerNode memberInitializer = Member_Initializer();
                if (memberInitializer == null) break;

                listOfNode.Add(memberInitializer);
                Token comma = analyzer.GetToken();

                if (comma == null || comma.GetTokenType() != TokenType.Operator || !comma.value.Equals(Operators.OP.opComma))
                {
                    analyzer.StepBack();
                    break;
                }

                listOfNode.Add(new OperatorNode() { token = comma });
            }

            return listOfNode;
            //can be a null !!!
        }

        private MemberInitializerNode Member_Initializer()
        {
            NodeIdentificator identificator = Parse_Identificator();
            Token equals = analyzer.GetToken();

            if (equals == null || equals.GetTokenType() != TokenType.Operator || !equals.value.Equals(Operators.OP.opEquals))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected \"=\", but get " + ((equals == null) ? "Null reference Exception" : equals.ToString()));

            return new MemberInitializerNode() { token = equals, children = new List<SyntaxisNode>() { identificator, Initializer_Value() } };
        }

        /// <summary>
        /// Проделываем небольшой трюк по предопределению символа {, который
        /// влияет на выбор инициализатора!!!
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Initializer_Value()
        {
            Token tok = analyzer.GetToken();
            analyzer.StepBack();

            if (tok != null && tok.GetTokenType() == TokenType.Operator && tok.value.Equals(Operators.OP.opLeftCurlyBracket))
                return ParseObjectOrCollectionInitializer();

            return ParseExpression();
        }

        private SyntaxisNode Collection_Initializer()
        {

        }
        #endregion

    }
}
