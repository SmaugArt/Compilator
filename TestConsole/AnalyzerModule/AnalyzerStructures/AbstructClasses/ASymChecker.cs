using System.Collections.Generic;

namespace AnalyzerCMD.AnalyzerModule.AnalyzerStructures.AbstructClasses
{
    abstract public class ASymChecker
    {
        protected List<SymToken> sToken;
        protected StatusKey otherSymType; //в случае не нахождения символа в списке токенов

        public TokenType type;//{ get; } //обычный возврат
        public TokenType endFileType; //что делать в случае окончания букв файла

        public ASymChecker(TokenType typeChecker, TokenType endLineTypeChecker, 
                                                        StatusKey otherWordStatus)
        {
            type = typeChecker;
            otherSymType = otherWordStatus;
            endFileType = endLineTypeChecker;
            sToken = new List<SymToken>();
        } //можно reader excel передавать для добавления элементов

        public StatusKey CheckSym(char sym)
        {
            foreach (SymToken item in sToken)
            {
                if (item.startSym <= sym && item.endSym >= sym)
                {
                    return item.status; 
                }
            }

            return otherSymType;
        }
    }
}
