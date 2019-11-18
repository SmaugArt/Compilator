using System;
using System.Collections.Generic;
using System.IO;
using AnalyzerCMD.AnalyzerModule;
using AnalyzerCMD.AnalyzerModule.AnalyzerStructures;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length < 1)
            {
                Console.WriteLine("Null argument!");
                return;
            }

            string pathToFile = args[0];
            List<Token> tokens = new List<Token>();

            if (!File.Exists(pathToFile))
            {
                Console.WriteLine("Incorrect file path:\"" + pathToFile + "\"");
                return;
            }

            using (StreamReader reader = new StreamReader(pathToFile))
            {
                Analyzer analyzer = new Analyzer(reader);

                while (analyzer.GetStatus() == AnalyzerStatus.OK)
                {
                    tokens.Add(analyzer.GetToken());
                }

                if (tokens[tokens.Count - 1] == null) tokens.RemoveAt(tokens.Count - 1);
            }

            foreach (Token item in tokens)
            {
                Console.WriteLine(item.ToString());
            }

            string d = "\n";
        }
    }
}
