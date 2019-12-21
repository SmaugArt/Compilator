using System.Collections.Generic;
using System.IO;
using Compilator.AnalyzerModule.AnalyzerStructures;
using Compilator.AnalyzerModule.AnalyzerStructures.AbstractClasses;
using Compilator.GeneralStructures;

namespace Compilator.AnalyzerModule
{
    public enum AnalyzerStatus
    {
        OK,
        Error,
        Empty //конец строки
    }
    public class Analyzer
    {
        private StreamReader reader;//поток данных с файла
        private CharPosition currentPos;
        private Dictionary<StatusKey, ASymChecker> sTokens; //подобие конечного автомата
        private AnalyzerStatus currentStatus;

        private List<AnalyzerStatus> previousStatus;
        private List<CharPosition> previousPos;
        public Analyzer(StreamReader reader)
        {
            this.reader = reader;
            currentPos = new CharPosition(0, 1, 1);
            currentStatus = AnalyzerStatus.OK;

            previousStatus = new List<AnalyzerStatus>(); // for stepBack
            previousPos = new List<CharPosition>();

            //особое условие следующей строки для ComentaryPart2: next line переводит состояние в ComentaryPart1, а в других случаях оканчивает с некоторым статусом complete

            sTokens = new Dictionary<StatusKey, ASymChecker>();
            sTokens.Add(StatusKey.Start, new StartStatus());
            sTokens.Add(StatusKey.SlashStatus, new SlashStatus());
            sTokens.Add(StatusKey.LineComentary, new LineComentary());
            sTokens.Add(StatusKey.LinesComentaryP1, new LinesComentaryPart1());
            sTokens.Add(StatusKey.LinesComentaryP2, new LinesComentaryPart2());
            sTokens.Add(StatusKey.MinusStatus, new MinusStatus());
            sTokens.Add(StatusKey.IntData, new IntData());
            sTokens.Add(StatusKey.DotStatus, new DotStatus());
            sTokens.Add(StatusKey.DoubleData, new DoubleData());
            sTokens.Add(StatusKey.DoubleDataP2, new DoubleDataPart2());
            sTokens.Add(StatusKey.DoubleDataP2b, new DoubleDataPart2b());
            sTokens.Add(StatusKey.DoubleDataP3, new DoubleDataPart3());
            sTokens.Add(StatusKey.Identificator, new Identificator());
            sTokens.Add(StatusKey.AmpersentStatus, new AmpersentStatus());
            sTokens.Add(StatusKey.PipeStatus, new PipeStatus());
            sTokens.Add(StatusKey.EqualsStatus, new EqualsStatus());
            sTokens.Add(StatusKey.Exclamation, new ExclamationStatus());
            sTokens.Add(StatusKey.StringData, new StringData());
            sTokens.Add(StatusKey.StringEcran, new StringEcran());
            sTokens.Add(StatusKey.RightAngleBracket, new RightAngleBracket());
            sTokens.Add(StatusKey.LeftAngleBracket, new LeftAngleBracket());
            sTokens.Add(StatusKey.CharDataP1, new CharDataP1());
            sTokens.Add(StatusKey.CharDataP2, new CharDataP2());
            sTokens.Add(StatusKey.CharEcran, new CharEcran());
            sTokens.Add(StatusKey.DoublePlus, new DoublePlus());
            sTokens.Add(StatusKey.DoubleMinus, new DoubleMinus());
        }

        virtual public Token GetToken()
        {
            SavePreviousStatus(); //for step back
            StatusKey statusOfAnalyz = StatusKey.Start;

            if (currentStatus != AnalyzerStatus.OK) return null;

            string newString = ""; //пустая строка

            int positionOfWord = currentPos.letter;
            int positionOfline = currentPos.linePos; //для того чтобы запомнить место начала этого символа или слова
            TokenType typeOfToken = TokenType.IncorrectData;

            while (statusOfAnalyz != StatusKey.Complete && statusOfAnalyz != StatusKey.Incorrect)
            {
                if (reader.Peek() == -1)
                {
                    currentStatus = AnalyzerStatus.Empty;

                    if (newString.Equals("")) { return null; }

                    typeOfToken = sTokens[statusOfAnalyz].endFileType; //присваиваем получившееся значение
                    break;

                } //конец файла

                char symbol = ReadSymbol();

                StatusKey nextStatus = sTokens[statusOfAnalyz].CheckSym(symbol);

                if (nextStatus == StatusKey.Complete)
                {
                    typeOfToken = sTokens[statusOfAnalyz].type;
                }

                //до этого момента мы еще не перевелись на новый символ!
                if (nextStatus == StatusKey.SymBack) //не записываем новый символ!
                {
                    typeOfToken = sTokens[statusOfAnalyz].type;
                    statusOfAnalyz = StatusKey.Complete;
                    GetBackSymbol();
                    break;
                }

                if (nextStatus == StatusKey.SymDotBack) //не записываем новый символ!
                {
                    typeOfToken = TokenType.IntData;
                    statusOfAnalyz = StatusKey.Complete;
                    GetBackSymbol();
                    GetBackSymbol();
                    newString = newString.Substring(0, newString.Length - 1); //удаляем последний символ

                    //reader.DiscardBufferedData();
                    //reader.BaseStream.Seek(currentPos.globalPos, SeekOrigin.Begin);//перемещаем указатель ///!!!!! возможно с нуля нужно считать global pos
                    StepBack();
                    break;
                }

                reader.Read();

                if (nextStatus == StatusKey.NewLine) //для коментариев
                {
                    nextStatus = StatusKey.LinesComentaryP1;
                    NextLine();
                }

                if (nextStatus == StatusKey.NewLineWithBreak) //для других данных (в самом начале аппарата конечного), когда string = ""
                {
                    nextStatus = StatusKey.Start;
                    NextLine();
                    positionOfWord = currentPos.letter; //обновляем
                    positionOfline = currentPos.linePos;
                    continue;
                }

                if (nextStatus == StatusKey.SymNext) //только для начала конечного аппарата
                {
                    nextStatus = StatusKey.Start;
                    positionOfWord = currentPos.letter; //обновляем
                    continue;
                }

                statusOfAnalyz = nextStatus;
                newString += symbol; //добавляем символ
                //проверка на разные статусы, которые не равны основным перечисленным в словаре
            }

            //проверка на символ \r в конце

            if (typeOfToken != TokenType.IncorrectData)
            {
                if (typeOfToken == TokenType.Identificator && KeyWords.CheckOnKeyWord(newString)) typeOfToken = TokenType.KeyWord;

                Token nToken = new Token(newString, positionOfline, positionOfWord, typeOfToken);

                if (!nToken.Finalyze())
                {
                    currentStatus = AnalyzerStatus.Error;
                    return nToken;
                }

                if (reader.Peek() == -1) currentStatus = AnalyzerStatus.Empty;

                return nToken;

            }
            else //if == incorrect
            {
                newString = DeleteLF(newString); //проверка на такие данные [...]+\r
                currentStatus = AnalyzerStatus.Error;
                return new Token(newString, positionOfline, positionOfWord, TokenType.IncorrectData);
            }
        }

        private char ReadSymbol()
        {
            char t = (char)reader.Peek();
            currentPos.globalPos++;
            currentPos.letter++;
            return t;
        }

        private void GetBackSymbol()
        {
            currentPos.globalPos--;
            currentPos.letter--;
        }

        private void NextLine()
        {
            currentPos.letter = 1;
            currentPos.linePos++;
        }
        public AnalyzerStatus GetStatus() { return currentStatus; }

        //добавить \n? Применимо к Incorrect Data
        /// <summary>
        /// удаление симвлов переноса каретки \r в конце строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string DeleteLF(string str)
        {
            if (str.Substring(str.Length - 1, 1).ToCharArray()[0] == '\r')
                return str.Remove(str.Length - 1, 1);

            return str;
        }

        /// <summary>
        /// Cохраняет текущий статус для возможности
        /// откатываться назад.
        /// </summary>
        private void SavePreviousStatus()
        {
            previousPos.Add(new CharPosition(currentPos.globalPos, currentPos.linePos, currentPos.letter));
            previousStatus.Add(currentStatus);
        }

        /// <summary>
        /// Возвращает результат выполнения анализатора на шаг назад, если это возможно.
        /// Возвращает true при удаче.
        /// </summary>
        /// <returns></returns>
        virtual public bool StepBack() //отменяет считывание токена
        {
            if (previousPos.Count <= 0 || previousStatus.Count <= 0) return false;

            currentPos = previousPos[previousPos.Count - 1];
            previousPos.RemoveAt(previousPos.Count - 1);

            currentStatus = previousStatus[previousStatus.Count - 1];
            previousStatus.RemoveAt(previousStatus.Count - 1);

            reader.DiscardBufferedData();
            reader.BaseStream.Seek(currentPos.globalPos, SeekOrigin.Begin);
            return true;
        }
    }
}
