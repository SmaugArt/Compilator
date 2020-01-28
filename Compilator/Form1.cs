using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Compilator.GeneralStructures;
using Compilator.AnalyzerModule;
using Compilator.SymanticModule;

namespace Compilator
{
    public partial class Form1 : Form
    {
        private string pathToProgrammFile;
        private string pathToPassFile; //путь к файлу проверки
        private List<string> programmLines; //строки программы
        private List<string> passLines;


        public Form1()
        {
            InitializeComponent();
            programmLines = new List<string>();
            passLines = new List<string>();
            textBox1.ScrollBars = ScrollBars.Both;
        }

        private void SearchPathButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FilePath1Box.Text = openFileDialog1.FileName;
                pathToProgrammFile = openFileDialog1.FileName;
            }
        }

        private void SearchButton2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                FilePatch2Box.Text = openFileDialog2.FileName;
                pathToPassFile = openFileDialog2.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            List<Token> tokens = new List<Token>();

            using (StreamReader reader = new StreamReader(pathToProgrammFile))
            {
                Analyzer analyzer = new AnalyzerWithoutCommentary(reader);

                SyntaxisModule.SyntaxisAnalyzer syn = new SyntaxisModule.SyntaxisAnalyzer(analyzer);

                    try
                    {
                        var tree = syn.SyntaxisParse();
                        new Symantic().Check(tree);
                        textBox1.Text = tree.ToString();
                    }
                    catch (Exception ex)
                    {
                        textBox1.Text = ex.Message;
                    }
            }

            ///Часть проверки
            //StringBuilder SB = new StringBuilder();
            //StringBuilder SB2 = new StringBuilder();

            //foreach (Token item in tokens)
            //{
            //    SB.AppendLine(item.ToString());
            //}

            //using (StreamReader reader = new StreamReader(pathToPassFile))
            //{
            //    while (reader.Peek() > -1) SB2.AppendLine(reader.ReadLine());
            //}
            //char[] s1 = SB.ToString().ToCharArray();
            //char[] s2 = SB2.ToString().ToCharArray();

            ///Проверка на несовпадающий символ
            ////for (int i = 0; i < s1.Length; i++)
            ////{
            ////    if (s1[i] != s2[i])
            ////    {
            ////        SB.AppendLine("Несовпадение символа:" + s1[i].ToString() + " и " + s2[i].ToString());
            ////    }
            ////}

            ///Запись строки в файл
            //using (StreamWriter writer = new StreamWriter(pathToPassFile))
            //{
            //    writer.Write(SB.ToString());
            //}

            //if (SB.ToString().Equals(SB2.ToString()))
            //{
            //    SB.AppendLine("-----------------------");
            //    SB.AppendLine("Проверка прошла успешно");
            //}
            //else
            //{
            //    SB.AppendLine("-----------------------");
            //    SB.AppendLine("Проверка завершилась провалом!");
            //}

            //textBox1.Text = SB.ToString();
        }
    }
}
