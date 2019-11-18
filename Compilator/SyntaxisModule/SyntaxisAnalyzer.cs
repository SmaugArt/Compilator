using Compilator.AnalyzerModule;
using Compilator.GeneralStructures;
using Compilator.SyntaxisModule.Structures.AbstractStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilator.SyntaxisModule
{
    public class SyntaxisAnalyzer
    {
        private Analyzer analyzer;
        public SyntaxisAnalyzer(Analyzer analyzer)
        {
            this.analyzer = analyzer;
        }

        public SyntaxisNode ParseExpr()
        {
            var left = ParseTerm(); //парсит терминал
            ParseExpr();
        }

        private SyntaxisNode ParseTerm()
        {

        }

        private SyntaxisNode ParseFactor()
        {
            Token t = analyzer.GetToken();
            //if (t==null) -достигли конца
            //if (t.GetTokenType == TokenType.Incorrect

            switch (t.GetTokenType())
            {
                case TokenType.Identificator: return new NodeIdentificator() { token = t };
                //?case TokenType.KeyWord: 
                //?case TokenType.Operator:
                case TokenType.CharData: return new NodeChar() { token = t };
                case TokenType.DoubleData: return new NodeDouble() { token = t };
                case TokenType.IntData: return new NodeInt() { token = t };
                case TokenType.StringData: return new NodeString() { token = t };
            }
        }
    }
}
