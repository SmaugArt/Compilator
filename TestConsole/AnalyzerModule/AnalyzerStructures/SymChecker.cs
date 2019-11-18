using AnalyzerCMD.AnalyzerModule.AnalyzerStructures.AbstructClasses;

namespace AnalyzerCMD.AnalyzerModule.AnalyzerStructures
{
    class StartStatus : ASymChecker
    {
        public StartStatus() : base(TokenType.Operator, TokenType.Null, StatusKey.Incorrect) =>
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '{', endSym = '{', status = StatusKey.Complete },
                new SymToken { startSym = '}', endSym = '}', status = StatusKey.Complete },
                new SymToken { startSym = '(', endSym = '(', status = StatusKey.Complete },
                new SymToken { startSym = ')', endSym = ')', status = StatusKey.Complete },
                new SymToken { startSym = '*', endSym = '*', status = StatusKey.Complete },
                new SymToken { startSym = '+', endSym = '+', status = StatusKey.Complete },
                new SymToken { startSym = ';', endSym = ';', status = StatusKey.Complete },
                new SymToken { startSym = ',', endSym = ',', status = StatusKey.Complete },
                new SymToken { startSym = '.', endSym = '.', status = StatusKey.Complete },
                new SymToken { startSym = '>', endSym = '>', status = StatusKey.RightAngleBracket },
                new SymToken { startSym = '<', endSym = '<', status = StatusKey.LeftAngleBracket },

                new SymToken { startSym = '/', endSym = '/', status = StatusKey.SlashStatus }, //деление или коментарий
                new SymToken { startSym = '-', endSym = '-', status = StatusKey.Complete },//MinusStatus },
                new SymToken { startSym = 'A', endSym = 'Z', status = StatusKey.Identificator },
                new SymToken { startSym = 'a', endSym = 'z', status = StatusKey.Identificator },
                new SymToken { startSym = '0', endSym = '9', status = StatusKey.IntData },
                new SymToken { startSym = '&', endSym = '&', status = StatusKey.AmpersentStatus },
                new SymToken { startSym = '|', endSym = '|', status = StatusKey.PipeStatus },
                new SymToken { startSym = '!', endSym = '!', status = StatusKey.Exclamation },
                new SymToken { startSym = '=', endSym = '=', status = StatusKey.EqualsStatus },
                new SymToken { startSym = '"', endSym = '"', status = StatusKey.StringData },
                new SymToken { startSym = '\'', endSym = '\'', status = StatusKey.CharDataP1 },

                new SymToken { startSym = ' ', endSym = ' ', status = StatusKey.SymNext },
                new SymToken { startSym = '\t', endSym = '\t', status = StatusKey.SymNext },
                new SymToken { startSym = '\r', endSym = '\r', status = StatusKey.SymNext }, //Идет в совокупности с символом переносам \n
                new SymToken { startSym = '\n', endSym = '\n', status = StatusKey.NewLineWithBreak }
            };
    }

    class SlashStatus : ASymChecker
    {
        public SlashStatus() : base(TokenType.Operator, TokenType.Operator, StatusKey.SymBack)=> // /
            sToken = new System.Collections.Generic.List<SymToken>() {
                new SymToken { startSym = '/', endSym = '/', status = StatusKey.LineComentary },
                new SymToken { startSym = '*', endSym = '*', status = StatusKey.LinesComentaryP1 }
            };
    }

    class LineComentary : ASymChecker {
        public LineComentary() : base(TokenType.Comentary, TokenType.Comentary, StatusKey.LineComentary) =>
            sToken.Add(new SymToken { startSym = '\r', endSym = '\r', status = StatusKey.SymBack }); //дошли до конца строки
    }

    class LinesComentaryPart1 : ASymChecker
    {
        public LinesComentaryPart1() : base(TokenType.Comentary, TokenType.IncorrectData, StatusKey.LinesComentaryP1) =>
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '\n', endSym = '\n', status = StatusKey.NewLine }, //дошли до конца строки
                new SymToken { startSym = '*', endSym = '*', status = StatusKey.LinesComentaryP2 } //дошли до конца строки
            }; 
    }

    class LinesComentaryPart2 : ASymChecker
    {
        public LinesComentaryPart2() : base(TokenType.Comentary, TokenType.IncorrectData, StatusKey.LinesComentaryP1) =>
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '/', endSym = '/', status = StatusKey.Complete }, //дошли до конца строки
                new SymToken { startSym = '\n', endSym = '\n', status = StatusKey.NewLine }
            };
    }

    class MinusStatus : ASymChecker
    {
        public MinusStatus() : base(TokenType.Operator, TokenType.Operator, StatusKey.SymBack) =>
            sToken.Add(new SymToken { startSym = '0', endSym = '9', status = StatusKey.IntData });
    }

    class IntData : ASymChecker
    {
        public IntData() : base(TokenType.IntData, TokenType.IntData, StatusKey.SymBack) =>
            sToken = new System.Collections.Generic.List<SymToken>()
            {
                new SymToken { startSym = '0', endSym = '9', status = StatusKey.IntData },
                new SymToken { startSym = '.', endSym = '.', status = StatusKey.DotStatus },
                new SymToken { startSym = 'e', endSym = 'e', status = StatusKey.DoubleDataP2 },
                new SymToken { startSym = 'E', endSym = 'E', status = StatusKey.DoubleDataP2 }
            };
    }

    class DotStatus : ASymChecker
    {
        public DotStatus() : base(TokenType.IncorrectData, TokenType.IncorrectData, StatusKey.SymDotBack) => ///!!!!!! 0.\r\n or 23.\r\n -> 0 и . или 23 и . //в конце если . -> 100% ошибка
            sToken.Add(new SymToken { startSym = '0', endSym = '9', status = StatusKey.DoubleData });
    }

    class DoubleData : ASymChecker
    {
        public DoubleData() : base(TokenType.DoubleData, TokenType.DoubleData, StatusKey.SymBack) =>
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '0', endSym = '9', status = StatusKey.DoubleData },
                new SymToken { startSym = 'e', endSym = 'e', status = StatusKey.DoubleDataP2 },
                new SymToken { startSym = 'E', endSym = 'E', status = StatusKey.DoubleDataP2 }
            };
    }

    class DoubleDataPart2 : ASymChecker
    {
        public DoubleDataPart2() : base(TokenType.IncorrectData, TokenType.IncorrectData, StatusKey.Incorrect) =>
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '0', endSym = '9', status = StatusKey.DoubleDataP3 },
                new SymToken { startSym = '-', endSym = '-', status = StatusKey.DoubleDataP2b }
            };
    }

    class DoubleDataPart2b : ASymChecker
    {
        public DoubleDataPart2b() : base(TokenType.IncorrectData, TokenType.IncorrectData, StatusKey.Incorrect)=>
            sToken.Add(new SymToken { startSym = '0', endSym = '9', status = StatusKey.DoubleDataP3 });
    }

    class DoubleDataPart3 : ASymChecker
    {
        public DoubleDataPart3() : base(TokenType.DoubleData, TokenType.DoubleData, StatusKey.SymBack)=>
            sToken.Add(new SymToken { startSym = '0', endSym = '9', status = StatusKey.DoubleDataP3 });
    }

    class Identificator : ASymChecker
    {
        public Identificator() : base(TokenType.Identificator, TokenType.Identificator, StatusKey.SymBack) =>
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '0', endSym = '9', status = StatusKey.Identificator },
                new SymToken { startSym = 'A', endSym = 'Z', status = StatusKey.Identificator },
                new SymToken { startSym = 'a', endSym = 'z', status = StatusKey.Identificator }
            };
    }

    class AmpersentStatus : ASymChecker
    {
        public AmpersentStatus() : base(TokenType.Operator, TokenType.Operator, StatusKey.SymBack) =>
            sToken.Add(new SymToken { startSym = '&', endSym = '&', status = StatusKey.Complete });
    }

    class PipeStatus : ASymChecker
    {
        public PipeStatus() : base(TokenType.Operator, TokenType.Operator, StatusKey.SymBack) =>
            sToken.Add(new SymToken { startSym = '|', endSym = '|', status = StatusKey.Complete });
    }

    class EqualsStatus : ASymChecker
    {
        public EqualsStatus() : base(TokenType.Operator, TokenType.Operator, StatusKey.SymBack) =>
            sToken.Add(new SymToken { startSym = '=', endSym = '=', status = StatusKey.Complete });
    }

    class ExclamationStatus : ASymChecker
    {
        public ExclamationStatus():base(TokenType.Operator, TokenType.Operator, StatusKey.SymBack) =>
            sToken.Add(new SymToken { startSym = '=', endSym = '=', status = StatusKey.Complete });
    }

    class StringData : ASymChecker
    {
        public StringData() : base(TokenType.StringData, TokenType.IncorrectData, StatusKey.StringData) =>
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '"', endSym = '"', status = StatusKey.Complete },
                new SymToken { startSym = '\r', endSym = '\r', status = StatusKey.Incorrect },
                new SymToken { startSym = '\\', endSym = '\\', status = StatusKey.StringEcran} //экранирование символа внутри строки
            }; //в случае переноса строки
    }

    class StringEcran : ASymChecker
    {
        public StringEcran() : base(TokenType.IncorrectData, TokenType.IncorrectData, StatusKey.Incorrect) => 
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '"', endSym = '"', status = StatusKey.StringData },
                new SymToken { startSym = 'n', endSym = 'n', status = StatusKey.StringData },
                new SymToken { startSym = 'r', endSym = 'r', status = StatusKey.StringData },
                new SymToken { startSym = 't', endSym = 't', status = StatusKey.StringData }
            };
    }

    class CharDataP1 : ASymChecker
    {
        public CharDataP1() : base(TokenType.IncorrectData, TokenType.IncorrectData, StatusKey.CharDataP2) =>
            sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken() { startSym = '\\', endSym = '\\', status = StatusKey.CharEcran },
                new SymToken() { startSym = '\r', endSym = '\r', status = StatusKey.Incorrect }, //эти символы плохи до симантического преобразования
                new SymToken() { startSym = '\n', endSym = '\n', status = StatusKey.Incorrect }, // '\r\n - ошибка ', а не '\r\n
                new SymToken() { startSym = '\t', endSym = '\t', status = StatusKey.Incorrect },
                new SymToken() { startSym = '\'', endSym = '\'', status = StatusKey.Incorrect }
            };
    }

    class CharEcran : ASymChecker
    {
        public CharEcran() : base(TokenType.IncorrectData, TokenType.IncorrectData, StatusKey.Incorrect) =>
        sToken = new System.Collections.Generic.List<SymToken>(){
                new SymToken { startSym = '\'', endSym = '\'', status = StatusKey.CharDataP2 },
                new SymToken { startSym = 'n', endSym = 'n', status = StatusKey.CharDataP2 },
                new SymToken { startSym = 'r', endSym = 'r', status = StatusKey.CharDataP2 },
                new SymToken { startSym = 't', endSym = 't', status = StatusKey.CharDataP2 }
            };
    }

    class CharDataP2 : ASymChecker
    {
        public CharDataP2() : base(TokenType.CharData, TokenType.IncorrectData, StatusKey.Incorrect) =>
        sToken.Add( new SymToken { startSym = '\'', endSym = '\'', status = StatusKey.Complete });
    }

    class LeftAngleBracket : ASymChecker
    {
        public LeftAngleBracket() : base(TokenType.Operator, TokenType.Operator, StatusKey.SymBack) =>
            sToken.Add(new SymToken { startSym = '=', endSym = '=', status = StatusKey.Complete });
    }

    class RightAngleBracket : ASymChecker
    {
        public RightAngleBracket() : base(TokenType.Operator, TokenType.Operator, StatusKey.SymBack)=>
            sToken.Add(new SymToken { startSym = '=', endSym = '=', status = StatusKey.Complete });
    }
}
