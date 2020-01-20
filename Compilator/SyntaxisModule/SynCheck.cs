using Compilator.GeneralStructures;

namespace Compilator.SyntaxisModule
{
    public static class SynCheck
    {
        public static bool TypeCheck(Token tok, params TokenType[] types)
        {
            if (tok == null) return false;

            foreach (TokenType type in types)
                if (tok.GetTokenType() == type)
                    return true;

            return false;
        }

        public static bool ValueCheck(Token tok, TokenType type, params object[] values)
        {
            if (tok == null) return false;
            if (tok.GetTokenType() != type) return false;

            foreach (object value in values)
                if (tok.value.Equals(value))
                    return true;

            return false;
        }
    }
}
