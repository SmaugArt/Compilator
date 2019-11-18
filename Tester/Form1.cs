using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Tester
{
    public partial class MainForm : Form
    {
        private string pathToProgrammEXE;
        private string[] programmPathes;
        private string[] trueAnswerPathes;

        private bool secOfProgramm;
        private bool secOfDirectory;
        private bool resultButton;

        private int positionOfArray;
        public MainForm()
        {
            InitializeComponent();
            secOfDirectory = false;
            secOfProgramm = false;
            positionOfArray = -1;
            resultButton = false;
        }
        private void SelectProgrammExeButton_Click(object sender, EventArgs e)
        {
            SelectProgrammExe();
        }
        private void SelectTestDirectoryButton_Click(object sender, EventArgs e)
        {
            SelectTestDirectory();
        }
        private void NextButton_Click(object sender, EventArgs e)
        {
            FunctionNext();
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            FunctionBack();
        }
        private void CheckButton_Click(object sender, EventArgs e)
        {
            FunctionCheck();
        }
        private void SelectProgrammExeButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(SelectProgrammExeButton, "Кнопка выбора exe файла для проверки");
        }
        private void SelectTestDirectoryButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(SelectTestDirectoryButton, "Кнопка выбора директории для проверки");
        }
        private void BackButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(BackButton, "Кнопка: переместиться назад в каталоге Program и Pass");
        }
        private void NextButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(NextButton, "Кнопка: переместиться вперед в каталоге Program и Pass");
        }
        private void CheckButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(CheckButton, "Кнопка: сверить выходные данные программы с правильными выходными данными");
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.A)
            {
                FunctionBack();
                Thread.Sleep(200);
            }

            if (keyData == Keys.Right || keyData == Keys.D)
            {
                FunctionNext();
                Thread.Sleep(200);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void SelectProgrammExe()
        {
            string pathToProgramm = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                pathToProgramm = openFileDialog1.FileName;

            if (!File.Exists(pathToProgramm))
            {
                ExceptionMessage("Exception: IncorrectFilePath", ref secOfProgramm);
                return;
            }

            pathToProgrammEXE = pathToProgramm;
            secOfProgramm = true;

            CheckStatusOfArgument();
        }
        private void SelectTestDirectory()
        {
            string pathToAnalyzarTestsDirectory = "";

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                pathToAnalyzarTestsDirectory = folderBrowserDialog1.SelectedPath;

            if (pathToAnalyzarTestsDirectory.Equals(""))
            {
                ExceptionMessage("Exception: IncorrectPathToTestsDirectory", ref secOfDirectory);
                return;
            }

            string[] dir = Directory.GetDirectories(pathToAnalyzarTestsDirectory, "*", SearchOption.AllDirectories);
            if (dir.Length > 2 || dir.Length < 2)
            {
                ExceptionMessage("Exception: IncorrectFolderValue", ref secOfDirectory);
                return;
            }

            string[] wayProgramm = dir[1].Split('\\');
            string[] wayAnswer = dir[0].Split('\\');

            int ProgrammTestPath = Array.IndexOf(wayProgramm, "Programm");
            int AnswerTestPath = Array.IndexOf(wayAnswer, "Pass");

            if (ProgrammTestPath == -1 || AnswerTestPath == -1)
            {
                ExceptionMessage("Exception: IncorrectFolderName", ref secOfDirectory);
                return;
            }

            programmPathes = Directory.GetFiles(dir[1], "*", SearchOption.AllDirectories);
            trueAnswerPathes = Directory.GetFiles(dir[0], "*", SearchOption.AllDirectories);

            if (programmPathes.Length != trueAnswerPathes.Length)
            {
                ExceptionMessage("Exception: IncorrectValueOfFilesInFolder", ref secOfDirectory);
                return;
            }

            positionOfArray = -1;
            secOfDirectory = true;
            CheckStatusOfArgument();

            textBox1.Text = "";
            AnswerBox.Text = "";
            ProgrammBox.Text = "";
        }
        private void ExceptionMessage(string message, ref bool param)
        {
            textBox1.Text = message;
            param = false;
            CheckStatusOfArgument();
        }
        private void FunctionBack()
        {
            if (!CheckStatusOfArgument()) return;

            if (positionOfArray - 1 < 0) return;

            positionOfArray--;

            AnswerBox.Text = ReadTXTToSTRING(trueAnswerPathes[positionOfArray]);
            ProgrammBox.Text = ReadTXTToSTRING(programmPathes[positionOfArray]);
            CheckProgramm(positionOfArray);
            resultButton = false;
        }
        private void FunctionNext()
        {
            if (!CheckStatusOfArgument()) return;

            if (positionOfArray + 1 >= programmPathes.Length) return;

            positionOfArray++;

            AnswerBox.Text = ReadTXTToSTRING(trueAnswerPathes[positionOfArray]);
            ProgrammBox.Text = ReadTXTToSTRING(programmPathes[positionOfArray]);
            CheckProgramm(positionOfArray);
            resultButton = false;
        }
        private void FunctionCheck()
        {
            if (!CheckStatusOfArgument()) return;

            if (positionOfArray >= trueAnswerPathes.Length || positionOfArray < 0) return;

            if (resultButton) return;

            #region GetAnswer
            string answerString = "";
            string outputProgrammText = textBox1.Text;

            using (StreamReader reader = new StreamReader(trueAnswerPathes[positionOfArray]))
            {
                while (reader.Peek() > -1)
                    answerString += reader.ReadLine() + "\r\n";
            }
            #endregion

            if (answerString.Replace("\r","").Equals(outputProgrammText))
            {
                textBox1.Text += "-----------------------\r\n";
                textBox1.Text += "Проверка прошла успешно!";
            }
            else
            {
                textBox1.Text += "-----------------------\r\n";
                textBox1.Text += "Проверка завершилась провалом!";
            }

            resultButton = true;
            HIIGLIGHTText();
        }

        /// <summary>
        /// Проверка на то: заданы ли все пути к исполняющей программе 
        /// и диреектории проверки или нет.
        /// </summary>
        /// <returns></returns>
        private bool CheckStatusOfArgument()
        {
            if (!secOfProgramm || !secOfDirectory)
            {
                ClearFilePathBoxes();
                BackButton.Enabled = false;
                NextButton.Enabled = false;
                CheckButton.Enabled = false;
                resultButton = false;
                return false;
            }

            BackButton.Enabled = true;
            NextButton.Enabled = true;
            CheckButton.Enabled = true;
            return true;

        }
        private void ClearFilePathBoxes()
        {
            FilePath1Box.Text = "Path to a programm .txt file";
            FilePath2Box.Text = "Path to a true answer .txt file";
        }

        /// <summary>
        /// Проверяет результат выполнения программы с заданными входными параметрами
        /// на соответствие с файлами правильных выводов.
        /// </summary>
        /// <param name="posOfArrElem">Позиция элемента в массиве, хранящего пути к файлам</param>
        private void CheckProgramm(int posOfArrElem)
        {
            textBox1.Text = "";

            try
            {
                string arguments = programmPathes[posOfArrElem];

                FilePath1Box.Text = programmPathes[posOfArrElem];
                FilePath2Box.Text = trueAnswerPathes[posOfArrElem];

                Process process1 = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = pathToProgrammEXE,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process1.ErrorDataReceived += Printer;
                process1.OutputDataReceived += Printer;
                //process1.Exited += ExitEvent;

                process1.Start();
                process1.BeginErrorReadLine();
                process1.BeginOutputReadLine();

                void Printer(object sender, DataReceivedEventArgs e)
                {
                    Trace.WriteLine(e.Data);

                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (e.Data == null) return;
                        if (e.Data.Equals("")) return;

                        byte[] bytes = Encoding.Default.GetBytes(e.Data);
                        textBox1.Text += Encoding.UTF8.GetString(bytes) + "\r\n";
                        HIIGLIGHTText();
                    }));
                }

                //void ExitEvent(object sender, EventArgs e)
                //{
                //    HIIGLIGHTText();
                //}
            }
            catch (Exception e)
            {
                textBox1.Text += e.Message;
            }
        }

        private void FilePath1Box_Click(object sender, EventArgs e)
        {
            if (positionOfArray > -1)
            {
                Process.Start(programmPathes[positionOfArray]);
            }
        }

        private void FilePath2Box_MouseClick(object sender, MouseEventArgs e)
        {
            if (positionOfArray > -1)
            {
                Process.Start(trueAnswerPathes[positionOfArray]);
            }
        }

        private string ReadTXTToSTRING(string pathToFile)
        {
            try
            {
                string str = "";

                using (StreamReader reader = new StreamReader(pathToFile))
                {
                    while (reader.Peek() > -1)
                        str += reader.ReadLine() + "\r\n";
                }

                return str;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Подсветка синтаксиса
        /// </summary>
        private void HIIGLIGHTText()
        {
            List<int> position = new List<int>() { 0 };
            while (position[position.Count - 1] != 0 || position.Count == 1)
            {
                position.Add(textBox1.Text.IndexOf("\n", position[position.Count - 1])+1);
            }
            position.RemoveAt(position.Count - 1);


            for (int i = 0; i < position.Count - 1; i++)
            {
                SEARCH_AND_HIIGLIGHT(textBox1, position[i], position[i + 1], Color.Black);

                int llPos = textBox1.Find(":", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, position[i], llPos, Color.Blue);

                int op = textBox1.Find("Operator:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, op, op + 9, Color.Green);

                int ident = textBox1.Find("Identificator:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, ident, ident + 14, Color.Green);

                int kw = textBox1.Find("KeyWord:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, kw, kw + 8, Color.Green);

                int str = textBox1.Find("StringData:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, str, str + 11, Color.Green);

                int ch = textBox1.Find("CharData:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, ch, ch + 9, Color.Green);

                int intD = textBox1.Find("IntData:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, intD, intD + 8, Color.Green);

                int douD = textBox1.Find("DoubleData:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, douD, douD + 11, Color.Green);

                int com = textBox1.Find("Comentary:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, com, com + 10, Color.Green);

                int incD = textBox1.Find("IncorrectData:", position[i], position[i + 1], RichTextBoxFinds.MatchCase);
                SEARCH_AND_HIIGLIGHT(textBox1, incD, incD + 14, Color.Red);

                if (str > 0) SEARCH_AND_HIIGLIGHT(textBox1, "\"", position[i], position[i + 1], Color.Brown);

                if (ch > 0) SEARCH_AND_HIIGLIGHT(textBox1, "'", position[i], position[i + 1], Color.Brown);

                if (incD > 0) SEARCH_AND_HIIGLIGHT(textBox1, incD + 14, position[i + 1], Color.Gray);
            }
            
        }

        /// <summary>
        /// Выделяет ту область richBox заданным цветом,
        /// которая указана в параметрах метода
        /// </summary>
        /// <param name="tb">RichTextBox linc</param>
        /// <param name="str">Строка, которая ищется в тексте для ограничения
        /// Например, в тексте: "hello word" можно выделить подстроку, ограниченную символами o.</param>
        /// <param name="starSearchIndex">Начальный индекс для поиска в тексте RichTextBox</param>
        /// <param name="endSearchIndex">Конечный индекс для поиска в тексте RichTextBox</param>
        /// <param name="col">Цвет выделения</param>
        public void SEARCH_AND_HIIGLIGHT(RichTextBox tb,string str,int starSearchIndex, int endSearchIndex, Color col)
        {
            int sPos = tb.Find(str, starSearchIndex, endSearchIndex, RichTextBoxFinds.MatchCase);
            int ePos = tb.Find(str, starSearchIndex, endSearchIndex, RichTextBoxFinds.Reverse);
            SEARCH_AND_HIIGLIGHT(tb, sPos, ePos+1, col);
        }
        public void SEARCH_AND_HIIGLIGHT(RichTextBox tb,int startIndex, int endIndex, Color col)
        {
            if (startIndex < 0 || startIndex > endIndex) return;
            tb.DeselectAll();
            tb.Select(startIndex, endIndex-startIndex);
            tb.SelectionColor = col;
        }
    }
}
