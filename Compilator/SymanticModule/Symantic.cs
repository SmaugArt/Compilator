using Compilator.AnalyzerModule.AnalyzerStructures;
using Compilator.SyntaxisModule.Structures;
using System.Collections.Generic;

namespace Compilator.SymanticModule
{
    public enum typeOfIdentify
    {
        Method,
        Parameter,
        Peremen,
        Class,
        Enum,
        Struct
    }

    public enum typeOfAccess
    {
        Local,
        Global
    }

    public class Identify
    {
        public string name;
        public typeOfIdentify type;
        public typeOfAccess access;

        public Identify() { }
        public Identify(SyntaxisNode node, typeOfIdentify type = typeOfIdentify.Parameter, typeOfAccess access = typeOfAccess.Global)
        {
            name = node.token.GetText();
            this.type = type;
            this.access = access;
        }
    }

    public class Symantic
    {

        private List<List<Identify>> levelIdentifiers;

        //если необходимо обновление levelIdentifiers с сохранением предыдущего значения
        private List<List<List<Identify>>> storage;

        List<SyntaxisNode> notFoundPerem;

        public Symantic()
        {
            levelIdentifiers = new List<List<Identify>>();
            storage = new List<List<List<Identify>>>();
            notFoundPerem = new List<SyntaxisNode>();
        }
        public void Check(SyntaxisNode node)
        {
            GlobalNodeCheck(node);
        }

        public void GlobalNodeCheck(SyntaxisNode node)
        {
            if (node.GetType() != typeof(GlobalNode))
                throw SymException.Show(SymExType.IncorrectNode, node);

            //создаем первый уровень
            levelIdentifiers.Add(new List<Identify>());

            foreach (SyntaxisNode item in node.children)
            {
                //usingNode не учитываем
                if (item.GetType() == typeof(UsingNode)) continue;

                if (item.GetType() == typeof(NamespaceDeclarationNode))
                {
                    storage.Add(levelIdentifiers);
                    levelIdentifiers = new List<List<Identify>>();

                    NamespaceDeclarationNodeCheck(item, 1);

                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                    continue;
                }

                //ищем идентификатор
                NodeIdentificator identify = SymMethod.SearchForType(item.children, typeof(NodeIdentificator)) as NodeIdentificator;
                if (!SymMethod.CheckUnique(levelIdentifiers, identify.token.GetText(), 1))
                    throw SymException.Show(SymExType.SimpleIdentify, identify);

                if (item.GetType() == typeof(ClassNode))
                {
                    levelIdentifiers[0].Add(new Identify(identify, typeOfIdentify.Class));
                    storage.Add(levelIdentifiers);
                    levelIdentifiers = new List<List<Identify>>();

                    ClassNodeCheck(item, 1, identify.token.GetText());

                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                    continue;
                }

                if (item.GetType() == typeof(EnumNode))
                {
                    levelIdentifiers[0].Add(new Identify(identify, typeOfIdentify.Enum));
                    //сохраняем текущий набор идентификаторов для последующего применения
                    //т.к. в enum все должно начинаться с 1-го уровня.
                    storage.Add(levelIdentifiers);
                    levelIdentifiers = new List<List<Identify>>();

                    EnumNodeCheck(item, 1);

                    //возвращаем все на место
                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                    continue;
                }

                if (item.GetType() == typeof(StructureNode))
                {
                    levelIdentifiers[0].Add(new Identify(identify, typeOfIdentify.Struct));

                    storage.Add(levelIdentifiers);
                    levelIdentifiers = new List<List<Identify>>();

                    StructureNodeCheck(item, 1, identify.token.GetText());

                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                }
            }

            levelIdentifiers.RemoveAt(0);
        }
        private void EnumNodeCheck(SyntaxisNode node, int level)
        {
            if (node.GetType() != typeof(EnumNode))
                throw SymException.Show(SymExType.IncorrectNode, node);

            levelIdentifiers.Insert(level - 1, new List<Identify>());

            foreach (SyntaxisNode item in node.children)
            {
                NodeIdentificator identify = item.children[0] as NodeIdentificator;

                if (!SymMethod.CheckUnique(levelIdentifiers, identify.ToString(), level))
                    throw SymException.Show(SymExType.SimpleIdentify, identify);

                levelIdentifiers[level - 1].Add(new Identify(identify, typeOfIdentify.Parameter));
            }
        }

        private void NamespaceDeclarationNodeCheck(SyntaxisNode node, int level)
        {
            if (node.GetType() != typeof(NamespaceDeclarationNode))
                throw SymException.Show(SymExType.IncorrectNode, node);

            levelIdentifiers.Insert(level - 1, new List<Identify>());
            NamespaceBodyNode _bodyNode = SymMethod.SearchForType(node.children, typeof(NamespaceBodyNode)) as NamespaceBodyNode;

            foreach (SyntaxisNode item in _bodyNode.children)
            {
                //usingNode не учитываем
                if (item.GetType() == typeof(UsingNode)) continue;

                if (item.GetType() == typeof(NamespaceDeclarationNode))
                {
                    storage.Add(levelIdentifiers);
                    levelIdentifiers = new List<List<Identify>>();

                    NamespaceDeclarationNodeCheck(item, 1);

                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                    continue;
                }

                //ищем идентификатор
                NodeIdentificator identify = SymMethod.SearchForType(item.children, typeof(NodeIdentificator)) as NodeIdentificator;
                if (!SymMethod.CheckUnique(levelIdentifiers, identify.token.GetText(), level))
                    throw SymException.Show(SymExType.SimpleIdentify, identify);

                if (item.GetType() == typeof(ClassNode))
                {
                    levelIdentifiers[0].Add(new Identify(identify, typeOfIdentify.Class));
                    storage.Add(levelIdentifiers);
                    levelIdentifiers = new List<List<Identify>>();

                    ClassNodeCheck(item, 1, identify.token.GetText());

                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                    continue;
                }

                if (item.GetType() == typeof(EnumNode))
                {
                    levelIdentifiers[0].Add(new Identify(identify, typeOfIdentify.Enum));
                    //сохраняем текущий набор идентификаторов для последующего применения
                    //т.к. в enum все должно начинаться с 1-го уровня.
                    storage.Add(levelIdentifiers);
                    levelIdentifiers = new List<List<Identify>>();

                    EnumNodeCheck(item, 1);

                    //возвращаем все на место
                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                    continue;
                }

                if (item.GetType() == typeof(StructureNode))
                {
                    levelIdentifiers[0].Add(new Identify(identify, typeOfIdentify.Struct));

                    storage.Add(levelIdentifiers);
                    levelIdentifiers = new List<List<Identify>>();

                    StructureNodeCheck(item, 1, identify.token.GetText());

                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                }
            }

            levelIdentifiers.RemoveAt(level - 1);
        }

        private void ClassNodeCheck(SyntaxisNode node, int level, string className)
        {
            //вконце проверка на непонятные переменные
            //заменяем новым массивом для этого класса, чтобы потом вернуть после проверки
            List<SyntaxisNode> notFoundReserve = notFoundPerem;
            notFoundPerem = new List<SyntaxisNode>();

            if (node.GetType() != typeof(ClassNode))
                throw SymException.Show(SymExType.IncorrectNode, node);

            levelIdentifiers.Insert(level - 1, new List<Identify>());
            ClassBodyNode _classBody = SymMethod.SearchForType(node.children, typeof(ClassBodyNode)) as ClassBodyNode;

            foreach (SyntaxisNode item in _classBody.children)
            {
                if (item.GetType() == typeof(ConstantDeclarationNode))
                {
                    int Pos = SymMethod.SearchPos(item.children, typeof(ConstantDeclaratorNode));
                    List<SyntaxisNode> list = SymMethod.Copy(item.children, Pos);
                    ConstantDeclaratorNodeCheck(list, level, className);
                    continue;
                }

                if (item.GetType() == typeof(FieldDeclarationNode))
                {
                    int Pos = SymMethod.SearchPos(item.children, typeof(VariableDeclaratorNode));
                    List<SyntaxisNode> list = SymMethod.Copy(item.children, Pos);
                    ConstantDeclaratorNodeCheck(list, level, className);//FieldDeclarationNodeCheck(list, level, className);
                    continue;
                }

                if (item.GetType() == typeof(ConstructorDeclarationNode))
                {
                    NodeIdentificator identify = SymMethod.SearchForType(item.children, typeof(NodeIdentificator)) as NodeIdentificator;

                    if (identify.token.GetText() != className)
                        throw new System.Exception("Неправильное имя конструктора: " + identify.ToString());

                    //ProgrammBlockNode pbNode 
                    var pb = SymMethod.SearchForType(item.children, typeof(ProgrammBlockNode));
                    if (pb != null) ProgrammBlockNodeCheck(pb, level + 1);
                }

                if (item.GetType() == typeof(DestructorDeclarationNode))
                {
                    NodeIdentificator identify = SymMethod.SearchForType(item.children, typeof(NodeIdentificator)) as NodeIdentificator;

                    if (identify.token.GetText() != className)
                        throw new System.Exception("Неправильное имя деструктора: " + identify.ToString());

                    //ProgrammBlockNode pbNode 
                    var pb = SymMethod.SearchForType(item.children, typeof(ProgrammBlockNode));
                    if (pb != null) ProgrammBlockNodeCheck(pb, level + 1);
                }

                if (item.GetType() == typeof(MethodDeclarationNode))
                {
                    NodeIdentificator identify = SymMethod.SearchForType(item.children, typeof(NodeIdentificator)) as NodeIdentificator;

                    if (identify.token.GetText() == className)
                        throw new System.Exception("Имя метода не должно совпадать с именем класса: " + identify.ToString());

                    //ProgrammBlockNode pbNode 
                    var pb = SymMethod.SearchForType(item.children, typeof(ProgrammBlockNode));
                    if (pb != null) ProgrammBlockNodeCheck(pb, level + 1);
                }
            }

            //проверка на ненайденные identify. Обязательно в конце класса или структуры
            foreach (SyntaxisNode item in notFoundPerem)
            {
                if (SymMethod.CheckUnique(levelIdentifiers, item.token.GetText(), level))
                    throw SymException.Show(SymExType.NonexistentIdentify, item);
            }
            //удаление данных

            notFoundPerem = notFoundReserve;
            levelIdentifiers.RemoveAt(level - 1);
        }

        private void ConstantDeclaratorNodeCheck(List<SyntaxisNode> list, int level, string className)
        {
            foreach (SyntaxisNode item in list)
            {
                NodeIdentificator id = item.children[0] as NodeIdentificator;

                if (!SymMethod.CheckUnique(levelIdentifiers, id.token.GetText(), level))
                    throw SymException.Show(SymExType.SimpleIdentify, id);

                if (id.token.GetText() == className)
                    throw new System.Exception("Имя переменной не должно совпадать с именем класса: " + id.ToString());

                levelIdentifiers[level - 1].Add(new Identify(id, typeOfIdentify.Peremen));

                CheckNoneExistentNode(item.children[1], level);
            }
        }

        private void ProgrammBlockNodeCheck(SyntaxisNode programmBlock, int level, bool createNewLevel = true)
        {
            if (programmBlock.GetType() != typeof(ProgrammBlockNode))
                throw SymException.Show(SymExType.IncorrectNode, programmBlock);

            if (createNewLevel) levelIdentifiers.Insert(level - 1, new List<Identify>());

            foreach (SyntaxisNode item in programmBlock.children)
            {
                if (item.GetType() == typeof(DeclarationStatementNode))
                {
                    int pos = SymMethod.SearchPos(item.children, typeof(LocalVariableDeclaratorNode));
                    List<SyntaxisNode> list = SymMethod.Copy(item.children, pos);
                    LocalVariableDeclaratorNodeCheck(list, level);
                    continue;
                }

                if (item.GetType() == typeof(ProgrammBlockNode))
                {
                    ProgrammBlockNodeCheck(item, level + 1);
                    continue;
                }

                if (item.GetType() == typeof(AssignmentNode))
                {
                    CheckNoneExistentNode(item.children[0], level);  //identify
                    CheckNoneExistentNode(item.children[1], level); //assigment part
                    continue;
                }

                if (item.GetType() == typeof(IfStatementNode))
                {
                    CheckNoneExistentNode(item.children[0], level); //условие

                    for (int i2 = 1; i2 < item.children.Count; i2++)
                    {
                        if (item.children[i2].GetType() != typeof(ProgrammBlockNode))
                            ProgrammBlockNodeCheck(
                                new ProgrammBlockNode()
                                {
                                    children = new List<SyntaxisNode>()
                                    {
                                        item.children[i2]
                                    }
                                },
                             level + 1);
                        else
                            ProgrammBlockNodeCheck(item.children[i2], level + 1);
                    }
                    continue;
                }

                if (item.GetType() == typeof(SwitchStatementNode))
                {
                    CheckNoneExistentNode(item.children[0], level); //переменная

                    SwitchBlockCheck(item.children[1], level + 1);
                }

                if (item.GetType() == typeof(WhileStatementNode))
                {
                    CheckNoneExistentNode(item.children[0], level);

                    ProgrammBlockNodeCheck(item.children[1], level + 1);
                }

                if (item.GetType() == typeof(DoStatementNode))
                {
                    ProgrammBlockNodeCheck(item.children[0], level + 1);

                    CheckNoneExistentNode(item.children[1], level);
                }

                //if (item.GetType() == typeof(ForStatementNode))
                //{
                //    int pos = 0;

                //    if (item.children[pos].GetType() != typeof(EmptyStatementNode))
                //    {
                //        ForInitializerCheck(item.children[pos], level);
                //        pos++;
                //    }

                //    pos++;

                //    if (item.children[pos].GetType() != typeof(EmptyStatementNode))
                //    {
                //        For_Condition(item.children[pos], level);
                //        pos++;
                //    }

                //    pos++;

                //    if (item.children[pos].GetType() == typeof(Statement_Expression_List))
                //    {
                //        StatementExpressionListCheck(item.children[pos], level);
                //        pos++;
                //    }

                //    if(item.children[pos].GetType() != typeof(ProgrammBlockNode))
                //        ProgrammBlockNodeCheck(new ProgrammBlockNode()
                //            {
                //                children = new List<SyntaxisNode>()
                //                    { item.children[pos] }
                //            }, 
                //            level + 1);
                //    else
                //        ProgrammBlockNodeCheck(item.children[pos], level + 1);
                //}

                //добавь continie для continue и break
                //для return

                //все остальное смотреть через CheckNoneExistentNode
            }

            if(createNewLevel) levelIdentifiers.RemoveAt(level - 1);
        }
        private void SwitchBlockCheck(SyntaxisNode node, int level)
        {
            if (node.GetType() != typeof(SwitchBlockNode))
                    throw SymException.Show(SymExType.IncorrectNode, node);

            levelIdentifiers.Insert(level - 1, new List<Identify>());

            foreach (SyntaxisNode item in node.children)
            {
                if (item.token.value.Equals(KeyWords.KW.kwCase))
                {
                    CheckNoneExistentNode(item.children[0], level); //переменная

                    ProgrammBlockNodeCheck(new ProgrammBlockNode()
                                           {
                                               children = new List<SyntaxisNode>()
                                               { item.children[1] }
                                           }, 
                        level, false);
                }
                else
                {
                    ProgrammBlockNodeCheck(new ProgrammBlockNode()
                    {
                        children = new List<SyntaxisNode>()
                                               { item.children[0] }
                    },
                       level, false);
                }
            }

            levelIdentifiers.RemoveAt(level - 1);
        }

        private void LocalVariableDeclaratorNodeCheck(List<SyntaxisNode> list, int level)
        {
            foreach (SyntaxisNode item in list)
            {
                NodeIdentificator id = item.children[0] as NodeIdentificator;

                if (!SymMethod.CheckUnique(levelIdentifiers, id.token.GetText(), level))
                    throw SymException.Show(SymExType.SimpleIdentify, id);

                levelIdentifiers[level - 1].Add(new Identify(id, typeOfIdentify.Peremen));

                CheckNoneExistentNode(item.children[1], level);
            }
        }

        private void StructureNodeCheck(SyntaxisNode node, int level, string structName)
        {
            List<SyntaxisNode> notFoundReserve = notFoundPerem;
            notFoundPerem = new List<SyntaxisNode>();

            if (node.GetType() != typeof(StructureNode))
                throw SymException.Show(SymExType.IncorrectNode, node);

            levelIdentifiers.Insert(level - 1, new List<Identify>());
            StructureBodyNode _classBody = SymMethod.SearchForType(node.children, typeof(StructureBodyNode)) as StructureBodyNode;

            foreach (SyntaxisNode item in _classBody.children)
            {
                if (item.GetType() == typeof(ConstantDeclarationNode))
                {
                    int Pos = SymMethod.SearchPos(item.children, typeof(ConstantDeclaratorNode));
                    List<SyntaxisNode> list = SymMethod.Copy(item.children, Pos);
                    ConstantDeclaratorNodeCheck(list, level, structName);
                    continue;
                }

                if (item.GetType() == typeof(FieldDeclarationNode))
                {
                    int Pos = SymMethod.SearchPos(item.children, typeof(VariableDeclaratorNode));
                    List<SyntaxisNode> list = SymMethod.Copy(item.children, Pos);
                    ConstantDeclaratorNodeCheck(list, level, structName);//FieldDeclarationNodeCheck(list, level, className);
                    continue;
                }

                if (item.GetType() == typeof(ConstructorDeclarationNode))
                {
                    NodeIdentificator identify = SymMethod.SearchForType(item.children, typeof(NodeIdentificator)) as NodeIdentificator;

                    if (identify.token.GetText() != structName)
                        throw new System.Exception("Неправильное имя конструктора: " + identify.ToString());

                    //ProgrammBlockNode pbNode 
                    var pb = SymMethod.SearchForType(item.children, typeof(ProgrammBlockNode));
                    if (pb != null) ProgrammBlockNodeCheck(pb, level + 1);
                }

                if (item.GetType() == typeof(MethodDeclarationNode))
                {
                    NodeIdentificator identify = SymMethod.SearchForType(item.children, typeof(NodeIdentificator)) as NodeIdentificator;

                    if (identify.token.GetText() == structName)
                        throw new System.Exception("Имя метода не должно совпадать с именем класса: " + identify.ToString());

                    //ProgrammBlockNode pbNode 
                    var pb = SymMethod.SearchForType(item.children, typeof(ProgrammBlockNode));
                    if (pb != null) ProgrammBlockNodeCheck(pb, level + 1);
                }
            }

            //проверка на ненайденные identify. Обязательно в конце класса или структуры
            foreach (SyntaxisNode item in notFoundPerem)
            {
                if (SymMethod.CheckUnique(levelIdentifiers, item.token.GetText(), level))
                    throw SymException.Show(SymExType.NonexistentIdentify, item);
            }
            //удаление данных

            notFoundPerem = notFoundReserve;
            levelIdentifiers.RemoveAt(level - 1);
        }

        /// <summary>
        /// Будут проверять на ошибку обращения к несуществующему параметру и т.д.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="level"></param>
        private void CheckNoneExistentNode(SyntaxisNode node, int level) { }
    }
}
