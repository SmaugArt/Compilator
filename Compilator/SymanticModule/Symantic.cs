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

                    StructureNodeCheck(item, 1);

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

                    StructureNodeCheck(item, 1);

                    levelIdentifiers.Clear();
                    levelIdentifiers = storage[storage.Count - 1];
                    storage.RemoveAt(storage.Count - 1);
                }

                levelIdentifiers.RemoveAt(level - 1);
            }
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
                    FieldDeclarationNodeCheck(list, level, className);
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
    }
}
