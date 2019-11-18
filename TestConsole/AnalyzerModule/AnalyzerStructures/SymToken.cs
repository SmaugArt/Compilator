namespace AnalyzerCMD.AnalyzerModule.AnalyzerStructures
{
    public enum StatusKey
    {
        Incorrect=-6,
        SymBack=-5, //вернуть на 1 символ назад и закончить на нем 
        SymDotBack=-4,
        SymNext=-3, //пройти вперед если встретили пробел
        NewLineWithBreak=-2,
        NewLine=-1, //перевести на новую строку
        Complete = 0, //успех - просто записать токен?
        Start, //статус старта
        Identificator, //Проверка на идентификатор
        StringData,
        MinusStatus,//-
        Exclamation, //!
        IntData,//-1 , 1
        DotStatus, // . 
        DoubleData, // -1.4 1.4
        DoubleDataP2, //-1.2e or 1.2e
        DoubleDataP2b, //-1.2e- or 1.2e-
        DoubleDataP3, //-1.2e-1 or 1.2e1
        AmpersentStatus,//&
        PipeStatus, //|
        SlashStatus,
        LineComentary, // // 
        LinesComentaryP1, // /**/
        LinesComentaryP2,
        EqualsStatus, //==
        LeftAngleBracket, //<
        RightAngleBracket,   //>
        CharDataP1,
        CharDataP2,
        CharEcran,
        StringEcran

    }
    public class SymToken
    {
        public char endSym;
        public char startSym;
        public StatusKey status;
    }
}
