using Compilator.AnalyzerModule;
using Compilator.AnalyzerModule.AnalyzerStructures;
using Compilator.GeneralStructures;
using Compilator.SyntaxisModule.Structures;
using System;
using System.Collections.Generic;

namespace Compilator.SyntaxisModule
{
    public enum ExpressionType { Assigment, Non_Assigment_Expression, Both }
    public enum ObjectOrCollectionType { ObjectInitializer, CollectionInitializer, Both }
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
        }

        /// <summary>
        /// Метод проверяет обязательное условие, 
        /// что первый токен отличен от пустого
        /// таким образом левое значения всегда будет ненулевым!!!
        /// </summary>
        /// <returns></returns>
        public SyntaxisNode SyntaxisParse()
        {
            SyntaxisNode node = Compilation_Unit();

            //Добавляю условие в центральный (главный) узел парсера
            //Если остались неразобранные значения, то это плохо
            var nullToken2 = analyzer.GetToken();
            if(!SynCheck.TypeCheck(nullToken2, TokenType.Null)) //теперь GetToken не возвращает null!!!
                throw new Exception("Unexpected Token! Message: " + nullToken2.ToString());

            return node;
        }

        /// <summary>
        /// Делаем Using directives без using_alias_directive
        /// После вызываем namespace_member_declaration
        /// Базовый Node не имеет начального значения
        /// </summary>
        /// <returns></returns>
        private GlobalNode Compilation_Unit()
        {
            //can be a null or have count equals zero
            List<SyntaxisNode> nodes = Using_Directives();

            if (nodes.Count == 0)        
                return new GlobalNode()
                    { children = Namespace_Member_Declarations() };
                
            var node = new GlobalNode() { children = nodes };
            node.children.AddRange(Namespace_Member_Declarations());

            return node;
        }

        private List<SyntaxisNode> Using_Directives()
        {
            List<SyntaxisNode> nodes = new List<SyntaxisNode>();

            while (true)
            {
                var node = Using_Directive();
                if (node == null) break;

                nodes.Add(node);
            }

            return nodes;
        }

        /// <summary>
        /// без using_alias_directive
        /// </summary>
        /// <returns></returns>
        private UsingNode Using_Directive()
        {
            Token usingTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(usingTok, TokenType.KeyWord, KeyWords.KW.kwUsing))
            {
                analyzer.StepBack();
                return null;
            }

            //продолжаем если все-таки используем using
            var node = Namespace_Name();

            Token commaDot = analyzer.GetToken();
            if(!SynCheck.ValueCheck(commaDot, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected \";\", but get " + commaDot.ToString());

            return new UsingNode() { token = usingTok, children = new List<SyntaxisNode>() { node } };
        }

        /// <summary>
        /// Возвращает список нодов пространства имен и всех "внутренностей"
        /// Внимание! Здесь опущен и переработан метод namespace_member_declaration
        /// </summary>
        /// <returns></returns>
        private List<SyntaxisNode> Namespace_Member_Declarations()
        {
            List<SyntaxisNode> namespaceDeclar = new List<SyntaxisNode>();
            while (true)
            {
                Token t = analyzer.Peek();

                if (SynCheck.TypeCheck(t, TokenType.Null)) break; //проверяем на непустые значения

                if (SynCheck.ValueCheck(t, TokenType.KeyWord, KeyWords.KW.kwNamespace))
                    namespaceDeclar.Add(Namespace_Declaration());
                else
                {
                    var typeDeclaration = Type_Declaration();
                    if (typeDeclaration.GetType() != typeof(EmptyNode))
                        namespaceDeclar.Add(typeDeclaration);
                    else
                        break;
                }
                    
            }

            return namespaceDeclar; //либо просто пустой список, либо ничего не добавится в случае отсутствия
        }

        /// <summary>
        /// Здесь Namespace_body приравниваем к Compilation_Unit, т.к. 
        /// реализуем и там и тут using и namespace
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Namespace_Declaration()
        {
            Token namespaceOp = analyzer.GetToken();

            if(!SynCheck.ValueCheck(namespaceOp, TokenType.KeyWord, KeyWords.KW.kwNamespace))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected keyword \"using\", but get: " + namespaceOp.ToString());

            var qualityIdNode = Qualified_Identifier();

            Token leftCyrkleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftCyrkleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"{\", but get: " + leftCyrkleBR.ToString());
            
            ///Global node конвертируем в NamespaceBodyNode
            var namespaceBodyNode = Compilation_Unit();//Namespace_Body();

            Token rightCyrkleBR = analyzer.GetToken();
            if (!SynCheck.ValueCheck(rightCyrkleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"{\", but get: " + rightCyrkleBR.ToString());

            NamespaceBodyNode node = new NamespaceBodyNode() { token = rightCyrkleBR, children = namespaceBodyNode.children };

            //возможна запятая после фигурных скобок
            Token commaDot = analyzer.GetToken();
            if (!SynCheck.ValueCheck(commaDot, TokenType.Operator, Operators.OP.opSemicolon)) ;
                analyzer.StepBack();

            return new NamespaceDeclarationNode() { token = namespaceOp, children = new List<SyntaxisNode>() { qualityIdNode, node } };
        }

        private SyntaxisNode Qualified_Identifier()
        {
            var node = Parse_Identificator();
            Token dot = analyzer.GetToken();

            if(!SynCheck.ValueCheck(dot, TokenType.Operator, Operators.OP.opDot))
            {
                analyzer.StepBack();
                return node;
            }

            return new QualifiedIdentifierNode() { token = dot, children =
                new List<SyntaxisNode>() { node, Qualified_Identifier() } };
        }

        /// <summary>
        /// Все структуры и типы, описываемые в КЛАССЕ!
        /// реализуем без: атрибутов
        /// Только Class, enum и structure
        /// [type] modificator + [type] + identificator + [type body]
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Type_Declaration()
        {
            Token modifier = analyzer.GetToken();

            if (!SynCheck.TypeCheck(modifier, TokenType.KeyWord) || !CheckModify(modifier))
            {
                analyzer.StepBack();
                modifier = null;
            }


            Token type = analyzer.GetToken();
            if(!SynCheck.TypeCheck(type, TokenType.KeyWord))
            {
                if(!SynCheck.ValueCheck(type, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected a [type] name!\r\nMessage: " + type.ToString());

                analyzer.StepBack();
                return new EmptyNode();
            }
                
            switch ((KeyWords.KW)type.value)
            {
                case KeyWords.KW.kwClass:
                    var CN = ParseClassComponents(modifier);
                    var classNode = new ClassNode() { token = type };
                    if (CN.children[0] != null) classNode.children.Add(CN.children[0]);
                    classNode.children.Add(new NodeIdentificator() { token = CN.token });
                    classNode.children.Add(Class_Body());
                    return classNode;

                case KeyWords.KW.kwEnum:
                    var CN2 = ParseEnumComponents(modifier);
                    var enumNode = new EnumNode() { token = type };
                    if (CN2.children[0] != null) enumNode.children.Add(CN2.children[0]);
                    enumNode.children.Add(new NodeIdentificator() { token = CN2.token });
                    enumNode.children.Add(Enum_Body());
                    return enumNode;

                case KeyWords.KW.kwStruct:
                    var CN3 = ParseStructComponents(modifier);
                    var structNode = new StructureNode() { token = type };
                    if (CN3.children[0] != null) structNode.children.Add(CN3.children[0]);
                    structNode.children.Add(new NodeIdentificator() { token = CN3.token });
                    structNode.children.Add(Struct_Body());
                    return structNode;

                default: throw new Exception("Unexpected Type name!\n\rMessage: " + type.ToString());
            }
        }

        /// <summary>
        /// Парсит компоненты класса.
        /// Передаются все токены, стоящие перед TypeName токеном!!!
        /// Без sealed, internal, 
        /// </summary>
        /// <param name="modifier">Обязательно нужно передать токен.
        /// Если null, то значит не было модификатора. Так может быть.</param>
        /// <param name="identificator"></param>
        /// <returns></returns>

        private TypeComponentsNode ParseTypesComponents(Token modifier, Token identificator, string typeName, params KeyWords.KW[] modifiers)
        {
            var mod = modifier;

            if (mod != null)
            {
                if (!SynCheck.TypeCheck(mod, TokenType.KeyWord))
                    throw SynException.ShowException(EXType.IncorrectToken, $"Expected a {typeName} modifier! Message: " + mod.ToString());

                int count = 0;
                foreach (KeyWords.KW item in modifiers)
                    if (mod.value.Equals(item))
                        break;
                    else
                        count += 1;

                if(count == modifiers.Length)
                     throw SynException.ShowException(EXType.IncorrectToken, $"Unexpected {typeName} modifier! Message: " + modifier.ToString());
            }

            Token ident = identificator;
            if (ident == null)
                ident = Parse_Identificator().token;
            else
            {
                if (ident.GetTokenType() != TokenType.Identificator)
                    throw SynException.ShowException(EXType.IncorrectToken, "Unexpected identificator! Message: " + ident.ToString());
            }

            //Код с дополнительными компонентами
            return new TypeComponentsNode()
            {
                token = ident,
                children = new List<SyntaxisNode>()
                {
                    new TypeModificatorNode(){token=modifier}
                }
            };
        }



        private TypeComponentsNode ParseClassComponents(Token modifier, Token identificator = null) => ParseTypesComponents(modifier, identificator, "Class", 
            KeyWords.KW.kwNew, KeyWords.KW.kwPublic, KeyWords.KW.kwProtected, KeyWords.KW.kwPrivate, KeyWords.KW.kwAbstract);

        private TypeComponentsNode ParseEnumComponents(Token modifier, Token identificator = null) => ParseTypesComponents(modifier, identificator, "Enum",
            KeyWords.KW.kwNew, KeyWords.KW.kwPublic, KeyWords.KW.kwProtected, KeyWords.KW.kwPrivate);
        
        private TypeComponentsNode ParseStructComponents(Token modifier, Token identificator = null) => ParseTypesComponents(modifier, identificator, "Struct",
            KeyWords.KW.kwNew, KeyWords.KW.kwPublic, KeyWords.KW.kwProtected, KeyWords.KW.kwPrivate);

        #region Class
        private ClassBodyNode Class_Body()
        {
            Token leftCyrkleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftCyrkleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"{\", but get: " + leftCyrkleBR.ToString());

            List<SyntaxisNode> classMember = Class_Member_Declarations();

            Token rightCyrkleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightCyrkleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"}\", but get: " + rightCyrkleBR.ToString());

            Token commaDot = analyzer.GetToken();
            if(!SynCheck.ValueCheck(commaDot, TokenType.Operator, Operators.OP.opSemicolon))
                analyzer.StepBack();

            ClassBodyNode BN = new ClassBodyNode()
            {
                token = leftCyrkleBR
            };
            BN.children.AddRange(classMember);
            return BN;
        }

        /// <summary>
        /// Переработан и осмыслено поведение
        /// Не использую: property_declaration, 
        /// event_declaration,
        /// indexer_declaration,
        /// operator_declaration,
        /// static_constructor_declaration.
        /// Приравниваю method_declaration к constructor_declaration
        /// </summary>
        /// <returns></returns>
        private List<SyntaxisNode> Class_Member_Declarations()
        {
            List<SyntaxisNode> nodes = new List<SyntaxisNode>();

            while (true)
            {
                Token rghtCurclyBR = analyzer.Peek();
                if(SynCheck.ValueCheck(rghtCurclyBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                    break;

                int analyzerStartPos = analyzer.stepBackCount;
                int classMemberDeclarationPos = analyzer.stepBackCount;
                string classMemberDeclarationError = "";

                try
                {
                    nodes.Add(Class_Member_Declaration());
                    continue;
                }
                catch(Exception ex)
                {
                    classMemberDeclarationPos = analyzer.stepBackCount;
                    classMemberDeclarationError = ex.Message;
                    
                    while (analyzer.stepBackCount> analyzerStartPos)
                        analyzer.StepBack();
                }

                try
                {
                    var typeDeclaration = Type_Declaration();
                    if (typeDeclaration.GetType() != typeof(EmptyNode))
                        nodes.Add(typeDeclaration);
                    else
                        break;
                }
                catch(Exception ex)
                {
                    if (analyzer.stepBackCount > classMemberDeclarationPos)
                        throw new Exception(ex.Message);
                    throw new Exception(classMemberDeclarationError);
                }
            }

            return nodes;
        }

        private DeclarationNode Class_Member_Declaration()
        {
            bool access;
            Token tilda = analyzer.GetToken();

            //+++++destructor
            if(SynCheck.ValueCheck(tilda, TokenType.Operator, Operators.OP.opTilda))
            {
                return Destructor_Declaration(tilda);
            }
            analyzer.StepBack();
            //-----destructor

            //парсим модификатор
            //Dictionary<string, bool> 
            var dic = Class_Member_Declaration_Modifier();

            ///+++++const
            Token _const = analyzer.Peek();
            if(SynCheck.ValueCheck(_const, TokenType.KeyWord, KeyWords.KW.kwConst))
            {
                ((Dictionary<string, bool>)dic[1]).TryGetValue("Constant", out access);
                if (!access)
                    throw SynException.ShowException(EXType.IncorrectNode, "Incorrect Constant modifier: " + ((Token)dic[0]).ToString());

                return Constant_Declaration((Token)dic[0]);
            }
            ///-----const

            //++++Constructor
            Token identify = analyzer.GetToken();
            Token leftBR = analyzer.Peek();
            analyzer.StepBack();

            if(SynCheck.TypeCheck(identify, TokenType.Identificator))
            {
                //true = Constructor_Declaration,false = field_declaration
                if(SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                {
                    ((Dictionary<string, bool>)dic[1]).TryGetValue("Constructor", out access);
                    if (!access)
                        throw SynException.ShowException(EXType.IncorrectNode, "Incorrect constructor modifier: " + ((Token)dic[0]).ToString());

                    return Constructor_Declaration((Token)dic[0]);
                }
                else
                {
                    ((Dictionary<string, bool>)dic[1]).TryGetValue("Field", out access);
                    if (!access)
                        throw SynException.ShowException(EXType.IncorrectNode, "Incorrect Field modifier: " + ((Token)dic[0]).ToString());

                    return Field_Declaration((Token)dic[0], Type_Parse_Level1());
                }
            }
            //----Constructor

            //+++++проверка на void -> проверка на method
            Token _void = analyzer.GetToken();
            if(SynCheck.ValueCheck(_void, TokenType.KeyWord, KeyWords.KW.kwVoid))
            {
                ((Dictionary<string, bool>)dic[1]).TryGetValue("Method", out access);
                if (!access)
                    throw SynException.ShowException(EXType.IncorrectNode, "Incorrect method modifier: " + ((Token)dic[0]).ToString());

                return Method_Declaration((Token)dic[0], new VoidTypeNode() { token = _void });
            }

            analyzer.StepBack();
            //-----
            var type = Type_Parse_Level1();

            //+++++Method
            var identify2 = analyzer.GetToken();
            leftBR = analyzer.Peek();//Token leftBR = analyzer.GetToken();
            analyzer.StepBack();

            if(!SynCheck.TypeCheck(identify2, TokenType.Identificator))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Identificator, bur get " + identify2.ToString());

            if(SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
            {
                ((Dictionary<string, bool>)dic[1]).TryGetValue("Method", out access);
                if (!access)
                    throw SynException.ShowException(EXType.IncorrectNode, "Incorrect method modifier: " + ((Token)dic[0]).ToString());

                return Method_Declaration((Token)dic[0], type);
            }

            ((Dictionary<string, bool>)dic[1]).TryGetValue("Field", out access);
            if (!access)
                throw SynException.ShowException(EXType.IncorrectNode, "Incorrect Field modifier: " + ((Token)dic[0]).ToString());

            return Field_Declaration((Token)dic[0], type);
        }

        //private Dictionary<string, bool> Class_Member_Declaration_Modifier()
        private object[] Class_Member_Declaration_Modifier()
        {
            Token tok = analyzer.GetToken();
            if(!SynCheck.TypeCheck(tok, TokenType.KeyWord))
            {
                analyzer.StepBack();
                return new object[] { null, Class_Member_Modifier_Constructor_Rules() };
            }


            switch ((KeyWords.KW)tok.value)
            {
                case KeyWords.KW.kwPrivate:
                    return new object[] { tok, Class_Member_Modifier_Constructor_Rules() };
                case KeyWords.KW.kwPublic:
                    return new object[] { tok, Class_Member_Modifier_Constructor_Rules() };
                case KeyWords.KW.kwAbstract:
                    return new object[] { tok, Class_Member_Modifier_Constructor_Rules(false, false, false) };
                case KeyWords.KW.kwOverride:
                    return new object[] { tok, Class_Member_Modifier_Constructor_Rules(false, false, false) };
                case KeyWords.KW.kwNew:
                    return new object[] { tok, Class_Member_Modifier_Constructor_Rules(false) };
                default:
                    analyzer.StepBack();
                    return new object[] { null, Class_Member_Modifier_Constructor_Rules() };
            }
        }

        private Dictionary<string, bool> Class_Member_Modifier_Constructor_Rules(bool constructor = true, bool constant = true, bool field = true, bool method = true)
        {
            return new Dictionary<string, bool>()
            {
                {"Constructor", constructor},
                {"Constant", constant },
                {"Field", field },
                {"Method", method },
            };
        }

        private ConstantDeclarationNode Constant_Declaration(Token modifier)
        {
            Token _const = analyzer.GetToken();
            if(!SynCheck.ValueCheck(_const, TokenType.KeyWord, KeyWords.KW.kwConst))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected KeyWord \"Const\"! But get " +  _const.ToString());

            var type = Type_Parse_Level1();
            var CD = Constant_Declarators();

            //не добавляю semilicon в node!!!
            Token semilicon = analyzer.GetToken();
            if(!SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\"! But get " + semilicon.ToString());

            ConstantDeclarationNode node = new ConstantDeclarationNode() { token = _const };

            if (modifier != null) node.children.Add(new TypeModificatorNode() { token = modifier });

            node.children.Add(type);
            node.children.AddRange(CD);
            return node;
        }

        private List<SyntaxisNode> Constant_Declarators()
        {
            List<SyntaxisNode> list = new List<SyntaxisNode>();
            while (true)
            {
                //проверка на запятую
                if (list.Count > 0)
                {
                    Token comma = analyzer.GetToken();
                    if(!SynCheck.ValueCheck(comma, TokenType.Operator, Operators.OP.opComma))
                    {
                        analyzer.StepBack();
                        break;
                    }
                }

                NodeIdentificator identify = Parse_Identificator();
                Token equals = analyzer.GetToken();

                if(!SynCheck.ValueCheck(equals, TokenType.Operator, Operators.OP.opEquals))
                    throw SynException.ShowException(EXType.IncorrectToken,"Expected Operator \"=\"! But get " + equals.ToString());

                var expression = ParseExpression();/// ParsePimaryExpression();
                list.Add(new ConstantDeclaratorNode() { token = equals, children = new List<SyntaxisNode>() { identify, expression } });
            }

            return list;
        }

        private FieldDeclarationNode Field_Declaration(Token modifier, SyntaxisNode type)
        {
            if (type == null)
                throw new Exception("Type of field declaration can not have a null value!");

            var VD = Variable_Declarators();

            //не добавляю semilicon в node!!!
            Token semilicon = analyzer.GetToken();
            if(!SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\"! But get " + semilicon.ToString());

            FieldDeclarationNode node = new FieldDeclarationNode() { };

            if (modifier != null) node.children.Add(new TypeModificatorNode() { token = modifier });

            node.children.Add(type);
            node.children.AddRange(VD);
            return node;
        }

        private List<SyntaxisNode> Variable_Declarators()
        {
            List<SyntaxisNode> list = new List<SyntaxisNode>();

            while (true)
            {
                //проверка на запятую
                if (list.Count > 0)
                {
                    Token comma = analyzer.GetToken();
                    if(!SynCheck.ValueCheck(comma, TokenType.Operator, Operators.OP.opComma))
                    {
                        analyzer.StepBack();
                        break;
                    }
                }

                var iden = Parse_Identificator();
                Token equals = analyzer.GetToken();
                if(!SynCheck.ValueCheck(equals, TokenType.Operator, Operators.OP.opEquals))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected a Operator \"=\", but get " +
                        ((equals == null) ? "Null reference exeption!" : equals.ToString()));

                SyntaxisNode variable_initializer = Variable_Initializer();
                list.Add(new VariableDeclaratorNode()
                {
                    token = equals,
                    children = new List<SyntaxisNode>()
                    {
                        iden,
                        variable_initializer
                    }
                });
            }

            return list;
        }

        /// <summary>
        /// Проверяем на {, т.к. должно однозначно идентифицировать операцию
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Variable_Initializer()
        {
            Token leftCyrcleBR = analyzer.Peek();
            if(SynCheck.ValueCheck(leftCyrcleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                return Array_Initializer();

            return ParseExpression();//ParsePimaryExpression();

        }

        private ConstructorDeclarationNode Constructor_Declaration(Token modify)
        {
            var memberName = Parse_Identificator();

            Token leftBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"(\", but get: " + leftBR.ToString());

            Token rightBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \")\", but get: " + rightBR.ToString());

            var MB = Constructor_Body();

            ConstructorDeclarationNode node = new ConstructorDeclarationNode();
            if (modify != null) node.children.Add(new TypeModificatorNode() { token = modify });
            node.children.Add(memberName);
            if (MB.GetType() != typeof(EmptyNode))
                node.children.Add(MB);

            return node;
        }

        private SyntaxisNode Constructor_Body()
        {
            Token semilicon = analyzer.GetToken();
            if(SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                return new EmptyNode();

            analyzer.StepBack();
            return Block_Parse();
        }

        /// <summary>
        /// Without formal_parameter_list, attributes,
        /// type_parameter_constraints_clauses and particles
        /// </summary>
        /// <param name="modify">Модификатор</param>
        /// <param name="type">Тип</param>
        /// <returns></returns>
        private MethodDeclarationNode Method_Declaration(Token modify, SyntaxisNode type)
        {
            var returnType = type;

            if (returnType == null) throw new Exception("Type of method declaration can not have a null value!");

            var memberName = Parse_Identificator();

            Token leftBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"(\", but get: " + leftBR.ToString());

            Token rightBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \")\", but get: " + rightBR.ToString());

            var MB = Method_Body();

            MethodDeclarationNode node = new MethodDeclarationNode();
            if (modify != null) node.children.Add(new TypeModificatorNode() { token = modify });
            node.children.Add(returnType);
            node.children.Add(memberName);
            if (MB.GetType() != typeof(EmptyNode)) node.children.Add(MB);

            return node;
        }

        private SyntaxisNode Method_Body()
        {
            Token semilicon = analyzer.GetToken();
            if(SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                return new EmptyNode();

            analyzer.StepBack();
            return Block_Parse();
        }

        /// <summary>
        /// tilda can not have a null value!
        /// без attributes и extern
        /// Destructor_Body -> приравниваем к Method_Body
        /// </summary>
        /// <param name="tilda"></param>
        /// <returns></returns>
        private DestructorDeclarationNode Destructor_Declaration(Token tilda)
        {
            if (tilda == null) throw new Exception("Destructor_Declaration can not have a null parameter!");

            var identifier = Parse_Identificator();

            Token leftBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"(\", but get: " + leftBR.ToString());

            Token rightBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \")\", but get: " + rightBR.ToString());

            var MB = Method_Body(); //==Destructor_Body
            DestructorDeclarationNode node = new DestructorDeclarationNode() { token = tilda };
            node.children.Add(identifier);
            if (MB.GetType() != typeof(EmptyNode)) node.children.Add(MB);

            return node;
        }
        #endregion

        #region Structure
        private StructureBodyNode Struct_Body()
        {
            Token leftCyrkleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftCyrkleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"{\", but get: " + leftCyrkleBR.ToString());

            List<SyntaxisNode> structMember = Struct_Member_Declarations();

            Token rightCyrkleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightCyrkleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"}\", but get: " + rightCyrkleBR.ToString());

            Token commaDot = analyzer.GetToken();
            if(!SynCheck.ValueCheck(commaDot, TokenType.Operator, Operators.OP.opSemicolon))
                analyzer.StepBack();

            StructureBodyNode BN = new StructureBodyNode() { token = leftCyrkleBR, children = structMember };

            return BN;
        }

        /// <summary>
        /// Type_Declaration вынес сюда, чтобы не мешался в
        /// struct_member_declaration
        /// </summary>
        /// <returns></returns>
        private List<SyntaxisNode> Struct_Member_Declarations()
        {
            List<SyntaxisNode> nodes = new List<SyntaxisNode>();

            while (true)
            {
                Token rghtCurclyBR = analyzer.Peek();
                if(SynCheck.ValueCheck(rghtCurclyBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                    break;

                int analyzerStartPos = analyzer.stepBackCount;
                int structMemberDeclarationPos = analyzer.stepBackCount;
                string structMemberDeclarationError = "";

                try
                {
                    nodes.Add(Struct_Member_Declaration());
                    continue;
                }
                catch (Exception ex)
                {
                    structMemberDeclarationPos = analyzer.stepBackCount;
                    structMemberDeclarationError = ex.Message;

                    while (analyzer.stepBackCount > analyzerStartPos)
                        analyzer.StepBack();
                }

                try
                {
                    var typeDeclaration = Type_Declaration();
                    if (typeDeclaration.GetType() != typeof(EmptyNode))
                        nodes.Add(typeDeclaration);
                    else
                        break;
                }
                catch (Exception ex)
                {
                    if (analyzer.stepBackCount > structMemberDeclarationPos)
                        throw new Exception(ex.Message);
                    throw new Exception(structMemberDeclarationError);
                }
            }

            return nodes;
        }

        //Отличием от Class_Member_Declaration является
        //отсутствие Destructor'a
        private DeclarationNode Struct_Member_Declaration()
        {
            bool access;

            //парсим модификатор
            //Dictionary<string, bool> 
            var dic = Class_Member_Declaration_Modifier();

            ///+++++const
            Token _const = analyzer.Peek();
            if(SynCheck.ValueCheck(_const, TokenType.KeyWord, KeyWords.KW.kwConst))
            {
                ((Dictionary<string, bool>)dic[1]).TryGetValue("Constant", out access);

                if (!access) throw SynException.ShowException(EXType.IncorrectNode, "Incorrect Constant modifier: " + ((Token)dic[0]).ToString());

                return Constant_Declaration((Token)dic[0]);
            }
            ///-----const

            //++++Constructor
            Token identify = analyzer.GetToken();
            Token leftBR = analyzer.Peek();
            analyzer.StepBack();
            if(SynCheck.TypeCheck(identify, TokenType.Identificator))
            {
                //true = Constructor_Declaration,false = field_declaration
                if(SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                {
                    ((Dictionary<string, bool>)dic[1]).TryGetValue("Constructor", out access);

                    if (!access) throw SynException.ShowException(EXType.IncorrectNode, "Incorrect constructor modifier: " + ((Token)dic[0]).ToString());

                    return Constructor_Declaration((Token)dic[0]);
                }
                else
                {
                    ((Dictionary<string, bool>)dic[1]).TryGetValue("Field", out access);

                    if (!access) throw SynException.ShowException(EXType.IncorrectNode, "Incorrect Field modifier: " + ((Token)dic[0]).ToString());

                    return Field_Declaration((Token)dic[0], Type_Parse_Level1());
                }
            }
            //----Constructor

            //+++++проверка на void -> проверка на method
            Token _void = analyzer.GetToken();
            if(SynCheck.ValueCheck(_void, TokenType.KeyWord, KeyWords.KW.kwVoid))
            {
                ((Dictionary<string, bool>)dic[1]).TryGetValue("Method", out access);

                if (!access) throw SynException.ShowException(EXType.IncorrectNode, "Incorrect method modifier: " + ((Token)dic[0]).ToString());

                return Method_Declaration((Token)dic[0], new VoidTypeNode() { token = _void });
            }
            analyzer.StepBack();
            //-----

            var type = Type_Parse_Level1();

            //+++++Method
            var identify2 = analyzer.GetToken();
            leftBR = analyzer.Peek();
            analyzer.StepBack();

            if(!SynCheck.TypeCheck(identify2, TokenType.Identificator))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Identificator, bur get " + identify2.ToString());

            if(SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
            {
                ((Dictionary<string, bool>)dic[1]).TryGetValue("Method", out access);

                if (!access) throw SynException.ShowException(EXType.IncorrectNode, "Incorrect method modifier: " + ((Token)dic[0]).ToString());

                return Method_Declaration((Token)dic[0], type);
            }

            ((Dictionary<string, bool>)dic[1]).TryGetValue("Field", out access);

            if (!access) throw SynException.ShowException(EXType.IncorrectNode, "Incorrect Field modifier: " + ((Token)dic[0]).ToString());

            return Field_Declaration((Token)dic[0], type);
        }
        #endregion

        #region Enum
        private EnumBodyNode Enum_Body()
        {
            Token leftCyrkleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftCyrkleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"{\", but get: " + leftCyrkleBR.ToString());

            List<SyntaxisNode> enumMember = Enum_Member_Declarations();

            Token rightCyrkleBR = analyzer.GetToken();           
            if(!SynCheck.ValueCheck(rightCyrkleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"}\", but get: " + rightCyrkleBR.ToString());

            Token commaDot = analyzer.GetToken();
            if(!SynCheck.ValueCheck(commaDot, TokenType.Operator, Operators.OP.opSemicolon))
                analyzer.StepBack();

            EnumBodyNode BN = new EnumBodyNode(){ token = leftCyrkleBR, children = enumMember};
            return BN;
        }

        private List<SyntaxisNode> Enum_Member_Declarations()
        {
            List<SyntaxisNode> nodes = new List<SyntaxisNode>();

            while (true)
            {
                int analyzerStartPos = analyzer.stepBackCount;

                try
                {
                    nodes.Add(Enum_Member_Declaration());
                }
                catch
                {
                    // Class_Member_Declaration = analyzer.stepBackCount;

                    while (analyzer.stepBackCount > analyzerStartPos)
                        analyzer.StepBack();
                    break;
                }

                Token comma = analyzer.GetToken();
                if(!SynCheck.ValueCheck(comma, TokenType.Operator, Operators.OP.opComma))
                {
                    analyzer.StepBack();
                    break;
                }
            }

            return nodes;
        }

        private IdentifierDeclarationListNode Enum_Member_Declaration()
        {
            var identify = Parse_Identificator();

            Token equalsTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(equalsTok, TokenType.Operator, Operators.OP.opEquals))
            {
                analyzer.StepBack();
                return new IdentifierDeclarationNode() { children = new List<SyntaxisNode>() { identify } };
            }

            var constantExpression = Constant_Expression();

            return new IdentifierDeclarationWithConstantExpressionNode()
            {
                token = equalsTok,
                children = new List<SyntaxisNode>()
                {
                    identify,
                    constantExpression
                }
            };
        }
        #endregion
        private SyntaxisNode Array_Initializer()
        {
            Token leftCyrcleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftCyrcleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"{\", but get: " + leftCyrcleBR.ToString());

            var VIL = Variable_Initializer_List();

            Token rightCyrcleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightCyrcleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected operator \"}\", but get: " + rightCyrcleBR.ToString());

            return new ArrayInitializerNode() { token = leftCyrcleBR, children = VIL };
        }

        private List<SyntaxisNode> Variable_Initializer_List()
        {
            List<SyntaxisNode> list = new List<SyntaxisNode>();

            while (true)
            {
                if (list.Count > 0)
                {
                    Token comma = analyzer.GetToken();
                    if(!SynCheck.ValueCheck(comma, TokenType.Operator, Operators.OP.opComma))
                    {
                        analyzer.StepBack();
                        break;
                    }
                }

                list.Add(Variable_Initializer());

            }

            return list;
        }


        #region Primary_Expression
        private SyntaxisNode ParsePimaryExpression() //without array_creation_expression
        {
            SyntaxisNode parsePNACE = Parse_Primary_No_Array_Creation_Expression();

            Token token = analyzer.GetToken();
            if (token == null) return parsePNACE;

            //+++++member_access
            if (SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opDot))
                return new MemberAccessNode()
                {
                    token = token,
                    children = new List<SyntaxisNode>()
                                 { parsePNACE, ParsePimaryExpression() }
                };
            //-----member_access

            //+++++post increment expression
            if (SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opIncrementExpression))
                return new PostIncrementNode() { token = token, children = new List<SyntaxisNode>() { parsePNACE } };
            //-----post increment expression

            //+++++post decrement expression
            if (SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opDecrementExpression))
                return new PostDecrementNode() { token = token, children = new List<SyntaxisNode>() { parsePNACE } };
            //-----post decrement expression

            //+++++element_access
            if (SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opLeftSquareBracket))
            {
                SyntaxisNode newNode = ParseExpression();

                Token token2 = analyzer.GetToken();
                ///////////////////if (token2 == null || token2.GetTokenType() != TokenType.Operator || (Operators.OP)token2.value != Operators.OP.opRightSquareBracket)
                if(!SynCheck.ValueCheck(token2, TokenType.Operator, Operators.OP.opRightSquareBracket))
                    throw new Exception("Expected Operator \"]\", but get: " + token2.ToString());

                return new ElementAccessNode() { token = token, children = new List<SyntaxisNode>() { parsePNACE, newNode } };
            }
            //-----element_access

            //+++++invocation expression
            if(SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opLeftParenthesis))//without parameters constructor
            {
                Token token2 = analyzer.GetToken();
                if(!SynCheck.ValueCheck(token2, TokenType.Operator, Operators.OP.opRightParenthesis))
                    throw new Exception("Expected Operator \")\", but get:" + token2.ToString());

                return new InvocationExpressionNode() { token = token, children = new List<SyntaxisNode>() { parsePNACE } };
            }
            //-----invocation expression

            analyzer.StepBack();
            return parsePNACE;
        }

        private SyntaxisNode Parse_Primary_No_Array_Creation_Expression() //like a Primary expression
        {
            Token t = analyzer.GetToken();
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
                    //++++object_creation_expression = StatementExpression.Object_Creation_Expression 
                    //Делаем без переменных, иначе придется реализовывать деревоы
                    if ((KeyWords.KW)t.value == KeyWords.KW.kwNew) {
                        SyntaxisNode type = Type_Parse_Level1(); //получаю type (Некий идентификатор или KeyWord)

                        Token opLeftParenthesis = analyzer.GetToken();
                        if(!SynCheck.ValueCheck(opLeftParenthesis, TokenType.Operator, Operators.OP.opLeftParenthesis))
                            throw new Exception("Expected \"(\", but get:" + opLeftParenthesis.ToString());

                        Token opRightParenthesis = analyzer.GetToken();
                        if (!SynCheck.ValueCheck(opRightParenthesis, TokenType.Operator, Operators.OP.opRightParenthesis))
                            throw new Exception("Expected \")\", but get:" + opRightParenthesis.ToString());

                        Token LeftCyrkleBrace = analyzer.GetToken();
                        if(!SynCheck.ValueCheck(LeftCyrkleBrace, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                        {
                            analyzer.StepBack();
                            return new ObjectCreationExpressionNode()
                            {
                                token = t
                            };
                        }

                        Token RightCyrkleBrace = analyzer.GetToken();
                        if (SynCheck.ValueCheck(RightCyrkleBrace, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                        {
                            return new ObjectCreationExpressionNode()
                            {
                                token = t
                            };
                        }

                        analyzer.StepBack();
                        analyzer.StepBack();
                        //параметры
                        SyntaxisNode par = ParseObjectOrCollectionInitializer();//ParseArgumentList(); //char data == standart datatype

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
                    if (t.value.Equals(Operators.OP.opLeftParenthesis)) //съедаем Expression
                    {
                        SyntaxisNode node = ParseExpression();
                        Token t2 = analyzer.GetToken();
                        if(SynCheck.ValueCheck(t2, TokenType.Operator, Operators.OP.opRightParenthesis))
                            return node;
                        throw new Exception("Expected Operator \")\", but get:" + t2.ToString());
                    }
                    break;
                    //----parenthesized_expression
            }

            throw SynException.ShowException(EXType.IncorrectToken, t.ToString());
        }

        private SyntaxisNode ParseExpression(ExpressionType type = ExpressionType.Both) //without lambda and query and assigmentExpression
        {
            int stepBackCount = analyzer.stepBackCount;

            if (type == ExpressionType.Non_Assigment_Expression)//conditional
                return Parse_Conditional_Expression();

            if (type == ExpressionType.Assigment)
                return Assignment_Parse();

            //в случае Both
            int ConditionDepth = analyzer.stepBackCount;
            string ExceptionMessage = "";

            try
            {
                return Parse_Conditional_Expression();
            }
            catch (Exception ex)
            {
                ConditionDepth = analyzer.stepBackCount;
                ExceptionMessage = ex.Message;

                while (analyzer.stepBackCount > stepBackCount)
                    analyzer.StepBack();
            }

            try
            {
                return Assignment_Parse();
            }
            catch (Exception ex) //вызываем правильный Exception среди двух обработанных методов!!!
            {
                int AssigmentDepth = analyzer.stepBackCount;

                if (AssigmentDepth > ConditionDepth)
                    throw new Exception("Error of parse Assigment expression.\r\nMessage: " + ex.Message);
                throw new Exception("Error of parse Conditional expression.\r\nMessage: " + ExceptionMessage);
            }

        }

        private SyntaxisNode Parse_Conditional_Expression()
        {
            var left = Null_Coalescing_Expression();

            Token token1 = analyzer.GetToken();
            if(SynCheck.ValueCheck(token1, TokenType.Operator, Operators.OP.opQuestionMark))
            {
                var middle = ParseExpression();
                Token tokenDoubleDot = analyzer.GetToken();

                if(!SynCheck.ValueCheck(tokenDoubleDot, TokenType.Operator, Operators.OP.opDoubleDot))
                    throw new Exception("Expected Operator \":\", but get:" + tokenDoubleDot.ToString());

                var right = ParseExpression();

                return new ConditionalExpressionNode() { token = token1, children = new List<SyntaxisNode>() { left, middle, right } };
            }

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode Null_Coalescing_Expression() => Conditional_Or_Expression();

        private SyntaxisNode Conditional_Or_Expression()  //||
        {
            var left = Conditional_And_Expression();

            Token operatorOr = analyzer.GetToken(); 
            if(SynCheck.ValueCheck(operatorOr, TokenType.Operator, Operators.OP.opOr))
                return new ConditionalOrExpressionNode() { token = operatorOr, children = new List<SyntaxisNode>() { left, Conditional_Or_Expression() } };

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode Conditional_And_Expression() // &&
        {
            var left = Inclusive_Or_Expression();
            Token operatorAnd = analyzer.GetToken();
            if (SynCheck.ValueCheck(operatorAnd, TokenType.Operator, Operators.OP.opAnd))
                return new ConditionalAndExpressionNode() { token = operatorAnd, children = new List<SyntaxisNode>() { left, Conditional_And_Expression() } };

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode Inclusive_Or_Expression() //|
        {
            var left = Exclusive_Or_Expression();
            Token opLogicalOr = analyzer.GetToken();

            if (SynCheck.ValueCheck(opLogicalOr, TokenType.Operator, Operators.OP.opLogicalOr))
                return new InclusiveOrExpressionNode() { token = opLogicalOr, children = new List<SyntaxisNode>() { left, Inclusive_Or_Expression() } };

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode Exclusive_Or_Expression() //^ = функциональое или (0^0=0,0^1=1,1^0,1^1=0)
        {
            var left = And_Expression();
            Token opExclusiveOr = analyzer.GetToken();
            if (SynCheck.ValueCheck(opExclusiveOr, TokenType.Operator, Operators.OP.opToDegree))
                return new ExclusiveOrExpressionNode() { token = opExclusiveOr, children = new List<SyntaxisNode>() { left, Exclusive_Or_Expression() } };

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode And_Expression()
        {
            var left = EqualityExpression();
            Token opLogicalAnd = analyzer.GetToken();
            if(SynCheck.ValueCheck(opLogicalAnd, TokenType.Operator, Operators.OP.opLogicalAnd))
                return new AndExpressionNode() { token = opLogicalAnd, children = new List<SyntaxisNode>() { left, And_Expression() } };

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode EqualityExpression() // ==, !=
        {
            var left = Relational_Expression_IS_AS();
            Token opEqualOrNotEqual = analyzer.GetToken();
            if(SynCheck.ValueCheck(opEqualOrNotEqual, TokenType.Operator, Operators.OP.opDoubleEquals, Operators.OP.opNotEquals))
                return new EqualityExpressionNode() { token = opEqualOrNotEqual, children = new List<SyntaxisNode>() { left, EqualityExpression() } };

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode Relational_Expression_IS_AS() //избавление от лево-рекурсивного зацикливания в Relational_Expression
        {
            var left = Relational_Expression();
            var kwISorAS = analyzer.GetToken();
            if (SynCheck.ValueCheck(kwISorAS, TokenType.KeyWord, KeyWords.KW.kwIs, KeyWords.KW.kwAs))
                return new RelationalExpressionNode() { token = kwISorAS, children = new List<SyntaxisNode>() { left, Type_Parse_Level1() } };

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode Relational_Expression()
        {
            var left = ShiftExpression();
            Token token = analyzer.GetToken();

            if(!SynCheck.TypeCheck(token, TokenType.Operator))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator" + token.ToString());

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
        private SyntaxisNode ShiftExpression() => Additive_Expression();

        private SyntaxisNode Additive_Expression() //+-
        {
            var left = Multiplicative_Expression();
            Token token = analyzer.GetToken();
            if(SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opPlus, Operators.OP.opMinus))
            {
                var right = Additive_Expression();
                return new BinaryOperationExpressionNode() { token = token, children = new List<SyntaxisNode>() { left, right } };
            }

            analyzer.StepBack();
            return left;
        }

        private SyntaxisNode Multiplicative_Expression() //*,/ without a %
        {
            var left = Unary_Expression_Primary_Part();
            Token token = analyzer.GetToken();
            if(SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opAsterisk, Operators.OP.opRightSlash))
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
        /// DLC 17.01.2020!!! Из-за того, что type_cast == parenthesized_expression, если
        /// встречается идентификатор, то делаем try catch
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Unary_Expression_Primary_Part()
        {
            int startPos = analyzer.stepBackCount;
            int lastPos = analyzer.stepBackCount;
            string messageUnary_Expression = "";

            try
            {
                var node = Unary_Expression();
                if (node != null) return node;
            }
            catch (Exception ex)
            {
                messageUnary_Expression = ex.Message;
                lastPos = analyzer.stepBackCount;
                while (analyzer.stepBackCount > startPos)
                    analyzer.StepBack();
            }

            try
            {
                return ParsePimaryExpression();
            }
            catch(Exception ex)
            {
                if (analyzer.stepBackCount > lastPos)
                    throw new Exception(ex.Message);
                throw new Exception(messageUnary_Expression);
            }
        }

        private ExpressionNode Unary_Expression()
        {
            Token token = analyzer.GetToken();
            if(SynCheck.TypeCheck(token, TokenType.Operator))
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
                        //+++Чтобы не было противоречий между type_cast и parenthesized_expression
                        int lastPos = analyzer.stepBackCount;
                        SyntaxisNode type = null;
                        try
                        {
                            type = Type_Parse_Level1();
                        }
                        catch
                        {
                            while (analyzer.stepBackCount > lastPos)
                                analyzer.StepBack();
                            break;
                        }
                        //---
                        
                        Token leftCyrklyBrasket = analyzer.GetToken();
                        if(!SynCheck.ValueCheck(leftCyrklyBrasket, TokenType.Operator, Operators.OP.opRightParenthesis))
                            throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \")\", but get: " + leftCyrklyBrasket.ToString());

                        return new CastEspression() { token = token, children = new List<SyntaxisNode>() { type, Unary_Expression_Primary_Part() } };
                }

            analyzer.StepBack();
            return null;
        }

        private SyntaxisNode Type_Parse_Level1() //убираем указатели type_unsafe
        {
            var Node = Type_Parse_Level2();
            Token token = analyzer.GetToken();

            //+++++nullable_type
            if(SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opQuestionMark))
                return new NullableTypeNode() { token = token, children = new List<SyntaxisNode>() { Node } };
            //-----nullable_type

            //+++++Array_type
            //реализуем многомерные массивы !?
            if (SynCheck.ValueCheck(token, TokenType.Operator, Operators.OP.opLeftSquareBracket))
            {
                List<SyntaxisNode> node = new List<SyntaxisNode>();
                Token coma = analyzer.GetToken();

                while(SynCheck.ValueCheck(coma, TokenType.Operator, Operators.OP.opComma))
                {
                    node.Add(new OperatorNode() { token = coma });
                    coma = analyzer.GetToken();
                }
                //analyzer.StepBack();

                Token rightSquareBR = coma;//analyzer.GetToken();
                if(!SynCheck.ValueCheck(rightSquareBR, TokenType.Operator, Operators.OP.opRightSquareBracket))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected \"]\", but get " + rightSquareBR.ToString());

                ArrayTypeNode atn = new ArrayTypeNode(){ token = token};
                atn.children.Add(Node);
                atn.children.AddRange(node);

                return atn;
            }
            //-----Array_type

            analyzer.StepBack();
            return Node;
        }

        private SyntaxisNode Type_Parse_Level2() //убираем указатели type_unsafe
        {
            Token token = analyzer.GetToken();
            if(!SynCheck.TypeCheck(token, TokenType.KeyWord, TokenType.Identificator))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Identificator, but get " + token.ToString());

            //в случае KyeWord
            if (token.GetTokenType() == TokenType.KeyWord)
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
            //type_parameter совмещаю с type_name, так как в обоих случаях используют identificator
            return Type_Name(token);
        }

        private NodeIdentificator Parse_Identificator(Token tokenRef = null)
        {
            Token token = (tokenRef == null) ? analyzer.GetToken() : tokenRef;
            if(!SynCheck.TypeCheck(token, TokenType.Identificator))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Identificator, but get: "+token.ToString());

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
            if(!SynCheck.TypeCheck(token, TokenType.Identificator))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Identificator, but get: " + token.ToString());

            Token dot = analyzer.GetToken();
            if(SynCheck.ValueCheck(dot, TokenType.Operator, Operators.OP.opDot))
                return new OperatorNode() { token = dot, children = new List<SyntaxisNode> { new NodeIdentificator() { token = token }, Namespace_Or_Type_Name() } };

            analyzer.StepBack();
            return new NodeIdentificator() { token = token };
        }

        /// <summary>
        /// without: %= ,&= ,|= ,^= ,<<= ,right_shift_assignment
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Assignment_Parse()
        {
            SyntaxisNode node = Unary_Expression_Primary_Part();

            Token assigmentToken = analyzer.GetToken();
            if(!SynCheck.TypeCheck(assigmentToken, TokenType.Operator))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator, but get: " + assigmentToken.ToString());

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
                    if(!SynCheck.ValueCheck(equals, TokenType.Operator, Operators.OP.opEquals))
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

        private SyntaxisNode ParseObjectOrCollectionInitializer(ObjectOrCollectionType type = ObjectOrCollectionType.Both)
        {
            if (type == ObjectOrCollectionType.CollectionInitializer)
                return Collection_Initializer();

            if (type == ObjectOrCollectionType.ObjectInitializer)
                return Object_Initializer();

            //both variant
            int lastStepBackCount = analyzer.stepBackCount;
            int Object_Initializer_depth = analyzer.stepBackCount;
            string object_initializer_message = "";

            try
            {
                return Object_Initializer();
            }
            catch (Exception ex)
            {
                Object_Initializer_depth = analyzer.stepBackCount;
                object_initializer_message = ex.Message;

                while(analyzer.stepBackCount > lastStepBackCount)
                    analyzer.StepBack();

            }

            try
            {
                return Collection_Initializer();
            }
            catch (Exception ex)
            {
                int collection_initializer_depth = analyzer.stepBackCount;

                if (collection_initializer_depth > Object_Initializer_depth)
                    throw new Exception("Error of parse Collection initializer.\r\nMessage: " + ex.Message);
                throw new Exception("Error of parse Object initializer.\r\nMessage: " + object_initializer_message);
            }
        }

        private SyntaxisNode Object_Initializer()
        {
            Token leftFiqureBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftFiqureBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"{\", but get: "+ leftFiqureBR.ToString());

            //can be a null!!!
            List<SyntaxisNode> node = Member_Initializer_List();
            Token rightFiqureBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightFiqureBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"}\", bur get: "+rightFiqureBR.ToString());

            if (node == null || node.Count == 0)
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
                if(!SynCheck.ValueCheck(comma, TokenType.Operator, Operators.OP.opComma))
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

            if(!SynCheck.ValueCheck(equals, TokenType.Operator, Operators.OP.opEquals))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"=\", but get " + equals.ToString());

            return new MemberInitializerNode() { token = equals, children = new List<SyntaxisNode>() { identificator, Initializer_Value() } };
        }

        /// <summary>
        /// Проделываем небольшой трюк по предопределению символа {, который
        /// влияет на выбор инициализатора!!!
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Initializer_Value()
        {
            Token tok = analyzer.Peek();
            if(SynCheck.ValueCheck(tok, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                return ParseObjectOrCollectionInitializer();

            return ParseExpression();
        }

        private SyntaxisNode Collection_Initializer()
        {
            Token leftFiqureBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftFiqureBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"{\", but get: " + leftFiqureBR.ToString());

            //can't be a null and have a count==0!!!
            List<SyntaxisNode> node = Element_Initializer_List();
            if (node == null || node.Count == 0)
                throw new Exception("Expected one ore more element initializing after: " + leftFiqureBR.ToString());

            Token rightFiqureBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightFiqureBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"}\", but get: " + rightFiqureBR.ToString());

            return new CollectionInitializerNode() { token = leftFiqureBR, children = node };
        }

        private List<SyntaxisNode> Element_Initializer_List()
        {
            List<SyntaxisNode> listOfNode = new List<SyntaxisNode>();

            while (true) //псевдо-бесконечный цикл
            {
                SyntaxisNode memberInitializer = Element_Initializer(); //будет зависить от типа полученного
                if (memberInitializer == null) break; ///?

                listOfNode.Add(memberInitializer);
                Token comma = analyzer.GetToken();

                if(!SynCheck.ValueCheck(comma, TokenType.Operator, Operators.OP.opComma))
                {
                    analyzer.StepBack();
                    break;
                }

                listOfNode.Add(new OperatorNode() { token = comma });
            }

            return listOfNode; //can't be a null !!!
        }

        /// <summary>
        /// Трюк с преждевременным просмотром токена
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Element_Initializer()
        {
            Token leftCyrkleBR = analyzer.GetToken();
            if(SynCheck.ValueCheck(leftCyrkleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
            {
                List<SyntaxisNode> expressionList = Expression_List();
                if (expressionList == null || expressionList.Count == 0)
                    throw new Exception("Expected one ore more expression after: " + leftCyrkleBR.ToString());

                Token rightCyrkleBR = analyzer.GetToken();
                if(!SynCheck.ValueCheck(rightCyrkleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"}\", but get: " + rightCyrkleBR.ToString());
                return new ExpressionListNode() { token = leftCyrkleBR, children = expressionList };
            }

            analyzer.StepBack();
            return ParseExpression(ExpressionType.Non_Assigment_Expression);
        }
        private List<SyntaxisNode> Expression_List()
        {
            List<SyntaxisNode> listOfNode = new List<SyntaxisNode>();

            while (true) //псевдо-бесконечный цикл
            {
                SyntaxisNode expression = ParseExpression(); //будет зависить от типа полученного
                if (expression == null) break;

                listOfNode.Add(expression);
                Token comma = analyzer.GetToken();

                if(!SynCheck.ValueCheck(comma, TokenType.Operator, Operators.OP.opComma))
                {
                    analyzer.StepBack();
                    break;
                }

                listOfNode.Add(new OperatorNode() { token = comma });
            }

            if (listOfNode.Count > 0 && listOfNode[listOfNode.Count - 1].token.GetTokenType() == TokenType.Operator)
                throw new Exception("Unexpected symbol " + listOfNode[listOfNode.Count - 1].token.ToString());

            return listOfNode;
        }
        #endregion

        #region Programm Block
        private SyntaxisNode Block_Parse()
        {
            Token leftCyrcleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftCyrcleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"{\", but get: " + leftCyrcleBR.ToString());

            var SL = Statement_List();

            Token rightCyrcleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightCyrcleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"}\", but get: " +rightCyrcleBR.ToString());

            return new ProgrammBlockNode() { token = leftCyrcleBR, children = SL };
        }

        /// <summary>
        /// labeled_statement == Метка (Не используем)
        /// declaration_statement == Переменная (Используем)
        /// embedded_statement == состояния для работы тела программы (Используем):
        /// ///Block (Use)
        /// ///empty_statement (Use)
        /// ///expression_statement (USe ALL !!!) --> начинай с тех, что без PrimaryExpression
        /// ///selection_statement (use)
        /// ///iteration_statement (While and for)
        /// ///jump_statement (break, continue, return)
        /// -------------------------------------------
        /// ///try_statement (non usable)
        /// ///checked_statement (non usable)
        /// ///unchecked_statement (non usable)
        /// ///lock_statement  (non usable)
        /// ///using_statement (non usable)
        /// ///yield_statement (non usable)
        /// ///embedded_statement_unsafe (non usable)
        /// ------------------------------------------------------------------------------
        /// ///DLC 17.01.2020 - Пришлось сделать через try -catch, т.к.
        /// ...Embedded_Statement.expression_statement.statement_expression.assignment
        /// ...схож с Declaration_Statement !!!
        /// </summary>
        /// <returns></returns>
        private List<SyntaxisNode> Statement_List()
        {
            List<SyntaxisNode> list = new List<SyntaxisNode>();

            ///Пусть будет declaration_statement, если получили
            ///Local variable_statement
            while (true)
            {
                //некая хитрость : )
                Token RightCyrcleBR = analyzer.Peek();
                if(SynCheck.ValueCheck(RightCyrcleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket)) break;

                if(SynCheck.ValueCheck(RightCyrcleBR, TokenType.KeyWord, KeyWords.KW.kwCase, KeyWords.KW.kwDefault)) break;
                
                SyntaxisNode LVT = null;
                int startAnPos = analyzer.stepBackCount;
                int maxAnPos = startAnPos;
                string DeclarationStatementError = "";

                try
                {
                    LVT = Local_Variable_Type();
                    list.Add(Declaration_Statement(LVT));
                    continue;
                }
                catch(Exception ex)
                {
                    maxAnPos = analyzer.stepBackCount;
                    DeclarationStatementError = ex.Message;
                    while (startAnPos < analyzer.stepBackCount)
                        analyzer.StepBack();
                }

                try
                {
                    list.Add(Embedded_Statement());
                }
                catch (Exception ex)
                {
                    if (analyzer.stepBackCount > maxAnPos)
                        throw new Exception(ex.Message);
                    throw new Exception(DeclarationStatementError);
                }
            }

            return list;
        }

        private SyntaxisNode Local_Variable_Type()
        {
            Token _var = analyzer.GetToken();
            if(SynCheck.ValueCheck(_var, TokenType.KeyWord, KeyWords.KW.kwVar))
                return new VarTypeNode() { token = _var };

            analyzer.StepBack();
            return Type_Parse_Level1();
        }

        private SyntaxisNode Local_Variable_Declaration_Level1(SyntaxisNode type)
        {
            if (type == null) throw new Exception("Destructor_Declaration can not have a null parameter!");

            DeclarationStatementNode node = new DeclarationStatementNode();
            node.children.Add(type);
            node.children.Add(Local_Variable_Declaration_Level2());
            return node;
        }

        private SyntaxisNode Local_Variable_Declaration_Level2()
        {
            SyntaxisNode identify = Parse_Identificator();
            Token equals = analyzer.GetToken();
            if(!SynCheck.ValueCheck(equals, TokenType.Operator, Operators.OP.opEquals))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"=\", but get: " + equals.ToString());

            var LVI = Local_Variable_Initializer();

            return new LocalVariableDeclaratorNode()
            {
                token = equals,
                children = new List<SyntaxisNode>()
                    {
                        identify,
                        LVI
                    }
            };
        }
        private SyntaxisNode Declaration_Statement(SyntaxisNode type)
        {
            if (type == null) throw new Exception("Destructor_Declaration can not have a null parameter!");

            List<SyntaxisNode> list = Local_Variable_Declarators();

            Token semilicon = analyzer.GetToken();
            if(!SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: " +semilicon.ToString());

            DeclarationStatementNode node = new DeclarationStatementNode();
            node.children.Add(type);
            node.children.AddRange(list);
            return node;
        }

        /// <summary>
        /// Содержит в себе local_variable_declarator
        /// </summary>
        /// <returns></returns>
        private List<SyntaxisNode> Local_Variable_Declarators()
        {
            List<SyntaxisNode> list = new List<SyntaxisNode>();

            while (true)
            {
                SyntaxisNode identify = Parse_Identificator();
                Token equals = analyzer.GetToken();
                if(!SynCheck.ValueCheck(equals, TokenType.Operator, Operators.OP.opEquals))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"=\", but get: " + equals.ToString());

                var LVI = Local_Variable_Initializer();
                list.Add(new LocalVariableDeclaratorNode()
                {
                    token = equals,
                    children = new List<SyntaxisNode>()
                    {
                        identify,
                        LVI
                    }
                });

                Token comma = analyzer.GetToken();
                if(!SynCheck.ValueCheck(comma, TokenType.Operator, Operators.OP.opComma))
                {
                    analyzer.StepBack();
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// Не используем local_variable_initializer_unsafe,
        /// Поэтому приравниваем к  Variable_Initializer
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode Local_Variable_Initializer() => Variable_Initializer();

        private SyntaxisNode Embedded_Statement()
        {
            //смотрим на то, что идет первым символом
            //если ничего не подходит (try type parse для переменных, иначе primaryExpression); 
            Token tok = analyzer.Peek();
            if(SynCheck.TypeCheck(tok, TokenType.Operator))
                switch ((Operators.OP)tok.value)
                {
                    case Operators.OP.opLeftCurlyBracket:
                        return Block_Parse();
                    case Operators.OP.opSemicolon:
                        return Empty_Statement();//важно записать для if и т.д.
                }

            if(SynCheck.TypeCheck(tok, TokenType.KeyWord))
                switch ((KeyWords.KW)tok.value)
                {
                    //selection_statement
                    case KeyWords.KW.kwIf:
                        return If_Statement();
                    case KeyWords.KW.kwSwitch:
                        return Switch_Statement();

                    //iteration_statement
                    case KeyWords.KW.kwWhile:
                        return While_Statement();
                    case KeyWords.KW.kwDo:
                        return Do_Statement();
                    case KeyWords.KW.kwFor:
                        return For_Statement();

                    //jump_statement
                    case KeyWords.KW.kwBreak:
                        return Break_Statement();
                    case KeyWords.KW.kwContinue:
                        return Continue_Statement();
                    case KeyWords.KW.kwReturn:
                        return Return_Statement();
                }

            return Expression_Statement();
        }

        private SyntaxisNode Empty_Statement()
        {
            Token tok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(tok, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: "+ tok.ToString());

            return new EmptyStatementNode() { token = tok };
        }

        private SyntaxisNode If_Statement()
        {
            Token tok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(tok, TokenType.KeyWord, KeyWords.KW.kwIf))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"if\", but get: " + tok.ToString());

            Token leftBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"(\", but get: " + leftBR.ToString());

            var boolean_expression = Boolean_Expression();

            Token rightBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \")\", but get: " + rightBR.ToString());

            var EmbExpression1 = Embedded_Statement();

            Token _ifTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(_ifTok, TokenType.KeyWord, KeyWords.KW.kwElse))
            {
                analyzer.StepBack();
                return new IfStatementNode()
                {
                    token = tok,
                    children = new List<SyntaxisNode>()
                    {
                        boolean_expression,
                        EmbExpression1
                    }
                };
            }

            var EmbExpression2 = Embedded_Statement();

            return new IfStatementNode()
            {
                token = tok,
                children = new List<SyntaxisNode>()
                    {
                        boolean_expression,
                        EmbExpression1,
                        EmbExpression2
                    }
            };
        }

        private SyntaxisNode Boolean_Expression() => ParseExpression();//ParsePimaryExpression();

        private SyntaxisNode Switch_Statement()
        {
            Token tok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(tok, TokenType.KeyWord, KeyWords.KW.kwSwitch))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"switch\", but get: " + tok.ToString());

            Token leftBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"(\", but get: " + leftBR.ToString());

            var expression = ParseExpression();//ParsePimaryExpression();

            Token rightBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \")\", but get: " + rightBR.ToString());           

            var switch_block = Switch_Block();

            if (switch_block.children.Count > 0)
                return new SwitchStatementNode()
                {
                    token = tok,
                    children = new List<SyntaxisNode>()
                    {
                        expression,
                        switch_block
                    }
                };

            return new SwitchStatementNode()
            {
                token = tok,
                children = new List<SyntaxisNode>(){ expression }
            };
        }

        private SyntaxisNode Switch_Block()
        {
            List<SyntaxisNode> list = new List<SyntaxisNode>();

            Token leftCyrcleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftCyrcleBR, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"{\", but get: " + leftCyrcleBR.ToString());

            while (true)
            {
                Token keyWord = analyzer.GetToken();
                if(!SynCheck.ValueCheck(keyWord, TokenType.KeyWord, KeyWords.KW.kwCase, KeyWords.KW.kwDefault))
                {
                    analyzer.StepBack();
                    break;
                }

                var constant_expression = keyWord.value.Equals(KeyWords.KW.kwCase) ? Constant_Expression() : null;

                Token dotDot = analyzer.GetToken();
                if(!SynCheck.ValueCheck(dotDot, TokenType.Operator, Operators.OP.opDoubleDot))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \":\", but get: "+ dotDot.ToString());

                var statement_list = Statement_List();

                list.Add(new SwitchLlabelNode() { token= keyWord });
                if (keyWord.value.Equals(KeyWords.KW.kwCase)) list[list.Count - 1].children.Add(constant_expression);
                list[list.Count - 1].children.AddRange(statement_list);
            }

            Token rightCyrcleBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightCyrcleBR, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"}\", but get: " + rightCyrcleBR.ToString());

            return new SwitchBlockNode() { token = leftCyrcleBR, children = list };
        }

        private SyntaxisNode Constant_Expression() => ParseExpression();//ParsePimaryExpression();

        private SyntaxisNode While_Statement()
        {
            Token whileTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(whileTok, TokenType.KeyWord, KeyWords.KW.kwWhile))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"while\", but get: " + whileTok.ToString());

            Token leftBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"(\", but get: " + leftBR.ToString());

            var expression = Boolean_Expression();

            Token rightBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \")\", but get: " + rightBR.ToString());

            var embededStatement = Embedded_Statement();

            return new WhileStatementNode()
            {
                token = whileTok,
                children = new List<SyntaxisNode>()
                {
                    expression,
                    embededStatement
                }
            };
        }

        private SyntaxisNode Do_Statement()
        {
            Token doTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(doTok, TokenType.KeyWord, KeyWords.KW.kwDo))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"do\", but get: " + doTok.ToString());

            var embededStatement = Embedded_Statement();

            //while
            Token whileTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(whileTok, TokenType.KeyWord, KeyWords.KW.kwWhile))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"while\", but get: " + whileTok.ToString());
            //---while

            Token leftBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"(\", but get: " + leftBR.ToString());

            var expression = Boolean_Expression();

            Token rightBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \")\", but get: " + rightBR.ToString());

            Token tokSemilikon = analyzer.GetToken();
            if(!SynCheck.ValueCheck(tokSemilikon, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: " + tokSemilikon.ToString());

            return new DoStatementNode()
            {
                token = doTok,
                children = new List<SyntaxisNode>()
                {
                    embededStatement,
                    expression
                }
            };
        }

        /// <summary>
        /// for( инициализация? ; состояние? ; Итератор?) embedded_statement
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode For_Statement()
        {
            bool initialize = false;
            SyntaxisNode initializerNode = null;
            bool conditional = false;
            SyntaxisNode conditionalNode = null;
            bool iterator = false;
            SyntaxisNode iteratorNode = null;

            Token forTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(forTok, TokenType.KeyWord, KeyWords.KW.kwFor))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"for\", but get: " +  forTok.ToString());

            Token leftBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(leftBR, TokenType.Operator, Operators.OP.opLeftParenthesis))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \"(\", but get: " + leftBR.ToString());

            Token semilikon1 = analyzer.GetToken();
            if(!SynCheck.ValueCheck(semilikon1, TokenType.Operator, Operators.OP.opSemicolon))
            {
                analyzer.StepBack();
                initialize = true;
                initializerNode = For_Initializer();

                semilikon1 = analyzer.GetToken();
                if(!SynCheck.ValueCheck(semilikon1, TokenType.Operator, Operators.OP.opSemicolon))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: " + semilikon1.ToString());
            }

            Token semilikon2 = analyzer.GetToken();
            if(!SynCheck.ValueCheck(semilikon2, TokenType.Operator, Operators.OP.opSemicolon))
            {
                analyzer.StepBack();
                conditional = true;
                conditionalNode = For_Condition();

                semilikon2 = analyzer.GetToken();
                if(!SynCheck.ValueCheck(semilikon2, TokenType.Operator, Operators.OP.opSemicolon))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: " + semilikon2.ToString());
            }

            Token rightBR = analyzer.GetToken();
            if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
            {
                analyzer.StepBack();
                iterator = true;
                iteratorNode = For_Iterator();

                rightBR = analyzer.GetToken();
                if(!SynCheck.ValueCheck(rightBR, TokenType.Operator, Operators.OP.opRightParenthesis))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: " + rightBR.ToString());
            }

            var embededStatement = Embedded_Statement();

            //return part
            ForStatementNode node = new ForStatementNode() { token = forTok };
            if (initialize) node.children.Add(initializerNode);
            if (conditional) node.children.Add(conditionalNode);
            if (iterator) node.children.Add(iteratorNode);
            node.children.Add(embededStatement);

            return node;
        }

        /// <summary>
        /// Сейчпс без Statement_Expression_List
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode For_Initializer()
        {
            ///+++local Variable Declaration
            int startPos = analyzer.stepBackCount;
            int maxPos = startPos;
            string localVariableMessage = "";

            try
            {
                var type = Local_Variable_Type();
                return Local_Variable_Declaration_Level1(type);//Declaration_Statement(type);
            }
            catch (Exception ex)
            {
                localVariableMessage = ex.Message;
                maxPos = analyzer.stepBackCount;
                while (startPos < analyzer.stepBackCount)
                    analyzer.StepBack();
            }
            //---
            //statement_expression_list
            try
            {
                return new Statement_Expression_List() { children = Statement_Expression_List() };
            }
            catch (Exception ex)
            {
                if (analyzer.stepBackCount > maxPos)
                    throw new Exception(ex.Message);
                else
                    throw new Exception(localVariableMessage);
            }
        }

        private SyntaxisNode For_Condition() => Boolean_Expression();

        private SyntaxisNode For_Iterator() => new Statement_Expression_List() { children = Statement_Expression_List() };

        private List<SyntaxisNode> Statement_Expression_List()
        {
            List<SyntaxisNode> list = new List<SyntaxisNode>();

            while (true)
            {
                list.Add(Statement_Expression());

                Token comma = analyzer.GetToken();
                if (comma == null || comma.GetTokenType() != TokenType.Operator ||
                    !comma.value.Equals(Operators.OP.opComma))
                {
                    analyzer.StepBack();
                    break;
                }
            }

            return list;
        }
        /// <summary>
        ///ExpressionNode return!
        ///++
        ///--
        ///new 
        ///try parse assigment -> ParseExpression(Assigment type)
        ///try parse PrimaryExpression:
        ///  ++
        ///  --
        ///  ()
        ///
        ///Проблема в том, что pre операции входят в unaryExpression, который
        /// относится к assignment операции. Решением будет попытка сразу
        /// использовать Assigment, а иначе все остальное !!!
        /// </summary>
        /// <returns></returns>
        private SyntaxisNode  Statement_Expression()
        {
            //+++Assigment part
            int startPos = analyzer.stepBackCount;
            int maxPos = analyzer.stepBackCount;

            try
            {
                return ParseExpression(ExpressionType.Assigment);
            }
            catch
            {
                maxPos = analyzer.stepBackCount;
                while (startPos < analyzer.stepBackCount)
                    analyzer.StepBack();
            }
            //---

            //+++ pre-expression
            Token tok = analyzer.GetToken();
            if(SynCheck.ValueCheck(tok, TokenType.Operator, Operators.OP.opIncrementExpression, Operators.OP.opDecrementExpression))
            {
                if (tok.value.Equals(Operators.OP.opIncrementExpression))//++
                    return new PreIncrementNode()
                    {
                        token = tok,
                        children = new List<SyntaxisNode>()
                        {
                            Unary_Expression_Primary_Part()
                        }
                    };
                else
                    return new PreDecrementNode()
                    {
                        token = tok,
                        children = new List<SyntaxisNode>()
                        {
                            Unary_Expression_Primary_Part()
                        }
                    };
            }
            //---
            //+++object_creation_expression
            if(SynCheck.ValueCheck(tok, TokenType.KeyWord, KeyWords.KW.kwNew))
            {
                SyntaxisNode type = Type_Parse_Level1(); //получаю type (Некий идентификатор или KeyWord)

                Token opLeftParenthesis = analyzer.GetToken();
                if(!SynCheck.ValueCheck(opLeftParenthesis, TokenType.Operator, Operators.OP.opLeftParenthesis))
                    throw new Exception("Expected Operator \"(\", but get: " + opLeftParenthesis.ToString());

                Token opRightParenthesis = analyzer.GetToken();
                if (!SynCheck.ValueCheck(opRightParenthesis, TokenType.Operator, Operators.OP.opRightParenthesis))
                    throw new Exception("Expected Operator \")\", but get: " + opRightParenthesis.ToString());

                Token LeftCyrkleBrace = analyzer.GetToken();
                if(!SynCheck.ValueCheck(LeftCyrkleBrace, TokenType.Operator, Operators.OP.opLeftCurlyBracket))
                    throw new Exception("Expected Operator \"{\", but get: " + LeftCyrkleBrace.ToString());

                //параметры
                SyntaxisNode par = ParseObjectOrCollectionInitializer();//ParseArgumentList(); //char data == standart datatype

                Token RightCyrkleBrace = analyzer.GetToken();
                /////////////////////////////////////if (RightCyrkleBrace.GetTokenType() != TokenType.Operator || (Operators.OP)RightCyrkleBrace.value != Operators.OP.opRightCurlyBracket)
                if(!SynCheck.ValueCheck(RightCyrkleBrace, TokenType.Operator, Operators.OP.opRightCurlyBracket))
                    throw new Exception("Expected Operator \"{\", but get: " + RightCyrkleBrace.ToString());

                return new ObjectCreationExpressionNode() { token = tok, children = new List<SyntaxisNode>() { par } };
            }
            analyzer.StepBack();
            //---
            //invocation_expression and post-Expression
            var pExpression = ParsePimaryExpression();
            Token tok2 = analyzer.GetToken();

            if(!SynCheck.TypeCheck(tok2, TokenType.Operator))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator token, but get: "+ tok2.ToString());

            switch ((Operators.OP)tok2.value)
            {
                case Operators.OP.opIncrementExpression:
                    return new PostIncrementNode() { token = tok2, children = new List<SyntaxisNode> { pExpression } };
                case Operators.OP.opDecrementExpression:
                    return new PostDecrementNode() { token = tok2, children = new List<SyntaxisNode> { pExpression } };
                default:
                    analyzer.StepBack();
                    return pExpression; //member access (invocation expression)
            }
        }

        private SyntaxisNode Break_Statement()
        {
            Token breakTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(breakTok, TokenType.KeyWord, KeyWords.KW.kwBreak) )
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"break\", but get "+ breakTok.ToString());

            Token semilicon = analyzer.GetToken();
            if (!SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: " + semilicon.ToString());

            return new BreakStatementNode() { token = breakTok };
        }

        private SyntaxisNode Continue_Statement()
        {
            Token continueTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(continueTok, TokenType.KeyWord, KeyWords.KW.kwContinue))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"continue\", but get " + continueTok.ToString());

            Token semilicon = analyzer.GetToken();
            if(!SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get " + semilicon.ToString());

            return new ContinueStatementNode() { token = continueTok };
        }

        private SyntaxisNode Return_Statement()
        {
            Token returnTok = analyzer.GetToken();
            if(!SynCheck.ValueCheck(returnTok, TokenType.KeyWord, KeyWords.KW.kwReturn))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Keyword \"return\", but get: " + returnTok.ToString());

            Token semilicon = analyzer.GetToken();
            if(!SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
            {
                analyzer.StepBack();
                var expression = ParseExpression();//ParsePimaryExpression();

                semilicon = analyzer.GetToken();
                if (!SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                    throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: " + semilicon.ToString());

                return new ReturnStatementNode() { token = returnTok, children = new List<SyntaxisNode>() { expression } };
            }

            return new ReturnStatementNode() { token = returnTok };

        }

        private SyntaxisNode Expression_Statement()
        {
            var statementExpression = Statement_Expression();
            Token semilicon = analyzer.GetToken();
            if(!SynCheck.ValueCheck(semilicon, TokenType.Operator, Operators.OP.opSemicolon))
                throw SynException.ShowException(EXType.IncorrectToken, "Expected Operator \";\", but get: " + semilicon.ToString());

            return new ExpressionStatementNode() { token = semilicon, children = new List<SyntaxisNode>() { statementExpression } };
        }
        #endregion

        /// <summary>
        /// Проверяет принадлежность токена к ключевому слову
        /// </summary>
        /// <param name="tok">Токен</param>
        /// <returns></returns>
        private bool CheckModify(Token tok)
        {
            if (!SynCheck.TypeCheck(tok, TokenType.KeyWord)) return false;

            switch ((KeyWords.KW)tok.value)
            {
                case KeyWords.KW.kwPrivate:
                case KeyWords.KW.kwPublic:
                case KeyWords.KW.kwAbstract:
                case KeyWords.KW.kwProtected:
                case KeyWords.KW.kwInternal:
                case KeyWords.KW.kwNew: return true;
                default: return false;
            }
        }

        
    }
}
