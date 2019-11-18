namespace Compilator.AnalyzerModule.AnalyzerStructures
{
    public class CharPosition
    {
        public int linePos; //локальный индекс строки
        public int letter; //локальный индекс буквы
        public int globalPos; //Номер символа ридера;

        public CharPosition(int globalCharPos, int linePos, int letter)
        {
            this.linePos = linePos;
            this.letter = letter;
            globalPos = globalCharPos;
        }
    }
}
