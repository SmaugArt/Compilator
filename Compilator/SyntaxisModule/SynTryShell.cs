using Compilator.SyntaxisModule.Structures;
using System;

namespace Compilator.SyntaxisModule
{
    //public enum typeTry
    //{
    //    Non_Parameters,
    //    One_Parameter,
    //    Two_Parameter
    //}

    public delegate SyntaxisNode NonParam();
    //public delegate SyntaxisNode OneParam(object obj);
    //public delegate SyntaxisNode TwoParam(object obj1, object obj2);

    public static class SynTryShell
    {
        public static bool TryThis(AnalyzerModule.Analyzer analyzer,NonParam _delegate,  out SyntaxisNode output,out int depth, out string error)
        {
            int startPos = analyzer.stepBackCount;
            
            try
            {
                output = _delegate.Invoke();
                depth = analyzer.stepBackCount;
                error = "";
                return true;
            }
            catch (Exception ex)
            {
                depth = analyzer.stepBackCount;
                error = ex.Message;
                output = null;
                while (analyzer.stepBackCount > startPos)
                    analyzer.StepBack();
                return false;
            }
        }
    }
}
