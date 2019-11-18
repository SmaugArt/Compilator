using System.Collections.Generic;

namespace AnalyzerCMD.AnalyzerModule.AnalyzerStructures
{
    public static class KeyWords
    {
        public enum KW
        {
            Empty = -1,
            kwAbstract,
            kwAs,
            kwBase,
            kwBool,
            kwBreak,
            kwByte,
            kwCase,
            kwCatch,
            kwChar,
            kwChecked,
            kwClass,
            kwConst,
            kwContinue,
            kwDecimal,
            kwDefault,
            kwDelegate,
            kwDo,
            kwDouble,
            kwElse,
            kwEnum,
            kwEvent,
            kwExplicit,
            kwExtern,
            kwFalse,
            kwFinally,
            kwFixed,
            kwFloat,
            kwFor,
            kwForeach,
            kwGoto,
            kwIf,
            kwImplicit,
            kwIn,
            kwInt,
            kwInterface,
            kwInternal,
            kwIs,
            kwLock,
            kwLong,
            kwNamespace,
            kwNew,
            kwNull,
            kwObject,
            kwOperator,
            kwOut,
            kwOverride,
            kwParams,
            kwPrivate,
            kwProtected,
            kwPublic,
            kwReadonly,
            kwRef,
            kwReturn,
            kwSbyte,
            kwSealed,
            kwShort,
            kwSizeof,
            kwStackalloc,
            kwStatic,
            kwString,
            kwStruct,
            kwSwitch,
            kwThis,
            kwThrow,
            kwTrue,
            kwTry,
            kwTypeof,
            kwUint,
            kwUlong,
            kwUnchecked,
            kwUnsafe,
            kwUshort,
            kwUsing,
            kwVirtual,
            kwVoid,
            kwVolatile,
            kwWhile,
            kwAdd,
            kwAlias,
            kwAscending,
            kwAsync,
            kwAwait,
            kwBy,
            kwDescending,
            kwDynamic,
            kwEquals,
            kwFrom,
            kwGet,
            kwGlobal,
            kwGroup,
            kwInto,
            kwJoin,
            kwLet,
            kwNameof,
            kwOn,
            kwOrderby,
            kwPartial,
            kwRemove,
            kwSelect,
            kwSet,
            kwUnmanaged,
            kwValue,
            kwVar,
            kwWhen,
            kwWhere,
            kwYield,
        }
        private static Dictionary<string, int> KeyWord;// = new Dictionary<string, int>()
        static KeyWords()
        {
            KeyWord = new Dictionary<string, int>();

            int i = 0;

            foreach (KW item in KW.GetValues(typeof(KW)))
            {
                string str = item.ToString().Substring(2).ToLower();
                KeyWord.Add(str, i);
                i++;
            }
        }

        public static bool CheckOnKeyWord(string str) =>
            KeyWord.ContainsKey(str);

        /// <summary>
        /// Функция нужна для преобразования строки в операторы для синтаксического оператора
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static KW GetKeyWord(string str)
        {
            int value;

            return (!KeyWord.TryGetValue(str, out value)) ? KW.Empty : (KW)value;
        }
    }
}
