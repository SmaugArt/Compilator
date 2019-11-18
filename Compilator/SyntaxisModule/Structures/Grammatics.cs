using Compilator.SyntaxisModule.Structures.AbstractStructures;
using System.Collections.Generic;
using Compilator.GeneralStructures;

namespace Compilator.SyntaxisModule.Structures
{
    public class Grammatics: AbstractGrammatic
    {
        public Grammatics() : base()
        {
            AddProduction(new List<GrammaticBody>()
            {
                new Terminal(SN.IdentificatorNode, TokenType.KeyWord, "+")
            });
        }
    }
}
