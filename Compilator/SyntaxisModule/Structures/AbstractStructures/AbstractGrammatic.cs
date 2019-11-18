using System.Collections.Generic;

namespace Compilator.SyntaxisModule.Structures.AbstractStructures
{
    public abstract class AbstractGrammatic
    {
        public List<List<GrammaticBody>> variantOfProduction; //вариант продукции

        public AbstractGrammatic()
        {
            variantOfProduction = new List<List<GrammaticBody>>();
        }

        public void AddProduction(List<GrammaticBody> data) => variantOfProduction.Add(data);
    }
}
