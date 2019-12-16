using Compilator.AnalyzerModule.AnalyzerStructures;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Compilator.GeneralStructures
{
    public enum TokenType
    {
        Operator, //=, ==, >=, <=, !, !=, *, +, /, -,|, ||, &&, &
        KeyWord, //ключевые слова: for, do, while, char, int, 
        Identificator, //hren123f, gf5, ggwp
        StringData, //строка 
        IntData,
        DoubleData,
        //BoolData,
        CharData,
        Comentary,
        IncorrectData,
        Null
    }
    public class Token
    {
        string text; //текст который получаем из строки
        public int line { get; private set; }
        public int letter { get; private set; }
        TokenType type;
        public object value { get; private set; }


        public Token(string text, int linePos, int letterPos, TokenType tokenType)
        {
            this.text = text;
            line = linePos;
            letter = letterPos;
            type = tokenType;
        }

        public override string ToString() => string.Format( //отвечает за переопределение метода вывода данных как String
            "{0},{1}: {2}",  // - по левому краю + - по правому выравнивания в символах
            line, letter, type.ToString() + ": " + text);

        /// <summary>
        /// преобразует строку по типу в какое-то значение
        /// Если преобразование прошло с ошибкой, то меняет тип токена
        /// на Incorrect
        /// </summary>
        public bool Finalyze()
        {
            switch (type)
            {
                case TokenType.CharData:
                    return ParseCharData();
                case TokenType.DoubleData:
                    return ParseDoubleData();
                case TokenType.IntData:
                    return ParseIntData();
                case TokenType.StringData:
                    return ParseStringData();
                case TokenType.KeyWord:
                    return ParseKeyWord();
                case TokenType.Operator:
                    return ParseOperator();
            }

            if (type == TokenType.IncorrectData) return false;

            return true; //для тех типов, которые не нужно преобразовывать
        }

        private bool ParseCharData()
        {
            try
            {

                string convertedStr = text.Substring(1, text.Length - 2);
                convertedStr = (convertedStr.Length > 1) ? Regex.Unescape(convertedStr) : convertedStr;

                value = char.Parse(convertedStr);
            }
            catch
            {
                type = TokenType.IncorrectData;
                return false;
            }

            return true;
        }

        private bool ParseDoubleData()
        {
            try
            {
                value = double.Parse(text, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                type = TokenType.IncorrectData;
                return false;
            }

            return true;
        }
        private bool ParseIntData()
        {
            try
            {
                value = int.Parse(text);
            }
            catch
            {
                type = TokenType.IncorrectData;
                return false;
            }

            return true;
        }
        private bool ParseStringData()
        {
            value = text.Remove(text.Length - 1, 1).Remove(0, 1);
            return true;
        }
        private bool ParseKeyWord()
        {
            KeyWords.KW type = KeyWords.GetKeyWord(text);
            value = type;

            if (type == KeyWords.KW.Empty)
            {
                this.type = TokenType.IncorrectData;
                return false;
            }

            return true;
        }
        private bool ParseOperator()
        {
            Operators.OP type = Operators.GetOperator(text);
            value = type;

            if (type == Operators.OP.Empty)
            {
                this.type = TokenType.IncorrectData;
                return false;
            }

            return true;
        }

        public string GetText() => text;
        public TokenType GetTokenType() => type;
        public object GetValue() => value; 
    }
}
