using Compilator.GeneralStructures;
using System.Collections.Generic;
using System.IO;

namespace Compilator.AnalyzerModule
{
    public class AnalyzerWithoutCommentary : Analyzer
    {
        private List<int> valueOfSteps;
        public AnalyzerWithoutCommentary(StreamReader reader) : base(reader) => valueOfSteps = new List<int>();

        public new int stepBackCount { get { return valueOfSteps.Count; } }
        public override Token GetToken()
        {
            Token token = base.GetToken();

            if (token == null) return null;

            valueOfSteps.Add(1);

            while (token.GetTokenType() == TokenType.Comentary)
            {
                token = base.GetToken();
                valueOfSteps[valueOfSteps.Count - 1]++;
                if (token == null) return null;
            }

            return token;
        }

        public override bool StepBack()
        {
            if (valueOfSteps.Count <= 0) return false;

            while (valueOfSteps[valueOfSteps.Count - 1] > 0)
            {
                base.StepBack();
                valueOfSteps[valueOfSteps.Count - 1]--;
            }

            valueOfSteps.RemoveAt(valueOfSteps.Count - 1);
            return true;
        }

        public override Token Peek()
        {
            var tok = this.GetToken();
            this.StepBack();
            return tok;
        }
    }
}
