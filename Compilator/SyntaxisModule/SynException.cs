using System;

namespace Compilator.SyntaxisModule
{
    public enum EXType
    {
        IncorrectNode,
        IncorrectToken,
        NullNode
    }
    public static class SynException
    {
        public static Exception ShowException(EXType type, string message="")
        {
            switch (type)
            {
                case EXType.IncorrectNode:
                    return new Exception("Incorrect node! Broken syntax rules.\r\nMessage: "+message);
                case EXType.IncorrectToken:
                    return new Exception("Incorrect token! Lexer error.\r\nMessage: "+message);
                case EXType.NullNode:
                    return new Exception("This node can not ba a null.\r\nMessage: " + message);
                default:
                    return new Exception("Message: " + message);
            }
        }
    }
}