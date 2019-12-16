using System.Collections.Generic;

namespace Compilator.AnalyzerModule.AnalyzerStructures
{
    public static class Operators
    {
        public enum OP
        {
            Empty =-1, //несовпадение
            opDot, //. //https://unicode-table.com/ru/#002C
            opPlus, //+
            opMinus, //-
            opAsterisk, // *
            opRightSlash, // /
            opComma, //,
            opLeftParenthesis, // (
            opRightParenthesis, // )
            opLeftCurlyBracket, // {
            opRightCurlyBracket, // }
            opExclamation, // !
            opEquals, // =
            opNotEquals, // !=
            opDoubleEquals, // ==
            opSemicolon, //;
            opOr, // ||
            opAnd, // &&
            opLogicalOr, // |
            opLogicalAnd, // &
            opLess, // <
            opGreater, // >
            opGreaterOrEquals, // >=
            opLessOrEquals, // <=
            opToDegree //^

        }

        private static Dictionary<string, OP> operatorsDictionary;

        static Operators()
        {
            operatorsDictionary = new Dictionary<string, OP>()
            {
                {".", OP.opDot },
                {"+", OP.opPlus},
                {"-", OP.opMinus},
                {"*", OP.opAsterisk},
                {"/", OP.opRightSlash},
                {",", OP.opComma},
                {"(", OP.opLeftParenthesis},
                {")", OP.opRightParenthesis},
                {"{", OP.opLeftCurlyBracket},
                {"}", OP.opRightCurlyBracket},
                {"!", OP.opExclamation},
                {"=", OP.opEquals},
                {"!=", OP.opNotEquals},
                {"==", OP.opDoubleEquals},
                {";", OP.opSemicolon},
                {"||", OP.opOr},
                {"&&", OP.opAnd},
                {"|", OP.opLogicalOr},
                {"&", OP.opLogicalAnd},
                {"<", OP.opLess},
                {">", OP.opGreater},
                {">=", OP.opGreaterOrEquals},
                {"<=", OP.opLessOrEquals},
                {"^", OP.opToDegree}
            };
        }

        public static OP GetOperator(string str)
        {
            OP typeOperators;

            return (!operatorsDictionary.TryGetValue(str, out typeOperators))? OP.Empty: typeOperators;
        }
    }
}
