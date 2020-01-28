using Compilator.SyntaxisModule.Structures;
using System;
using System.Collections.Generic;

namespace Compilator.SymanticModule
{
    public static class SymMethod
    {
        public static SyntaxisNode SearchForType(List<SyntaxisNode> items, Type type)
        {
            foreach (SyntaxisNode item in items)
                if (item.GetType() == type)
                    return item;

            return null;
        }

        public static SyntaxisNode SearchForType(List<SyntaxisNode> items, Type type, int pos)
        {
            List<SyntaxisNode> list = new List<SyntaxisNode>();

            for (int i = pos; i < items.Count; i++)
                list.Add(items[i]);

            return SearchForType(list, type);
        }

        /// <summary>
        /// Проверяет на уникальность идентификатор
        /// </summary>
        /// <param name="item">Уровни идентификаторов</param>
        /// <param name="identify">Переменная идентификатора</param>
        /// <param name="accessLevel">Уровень доступа. Например item имеет 3 уровня. 
        /// Следовательно, чем выше уровень, тем глубже спуск.</param>
        /// <returns></returns>
        public static bool CheckUnique(List<List<Identify>> item, string identify, int accessLevel)
        {
            for (int i = 0; i < accessLevel; i++)
            {
                foreach (Identify it in item[i])
                    if (it.name == identify)
                        return false;
            }

            return true;
        }

        /// <summary>
        /// Возвращает индекс первого вхождения типа,
        /// начиная отсчет с нуля!
        /// </summary>
        /// <param name="items"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int SearchPos(List<SyntaxisNode> items, Type type)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].GetType() == type)
                    return i;
            } 
              
            return -1;
        }

        public static List<SyntaxisNode> Copy(List<SyntaxisNode> list, int startPos, int endPos)
        {
            List<SyntaxisNode> newList = new List<SyntaxisNode>();

            for (int i = startPos; i <= endPos; i++)
            {
                if (i >= list.Count) break;
                newList.Add(list[i]);
            }

            return newList;
        }

        public static List<SyntaxisNode> Copy(List<SyntaxisNode> list, int startPos) => Copy(list, startPos, list.Count - 1);
    }
}
