using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        public static string GetIndexTableName(string objectName, string indexPropertyName)
        {
            var T = objectName.LettersAndNumbersOnly();
            var I = indexPropertyName.LettersAndNumbersOnly();

            var S = string.Format("{0}Idx{1}", T, I);

            if (S.Length <= 63) return S;

            return "";
        }

        public static string LettersAndNumbersOnly(this string s)
        {
            return s.ToCharArray().Where(Char.IsLetterOrDigit).Aggregate("", (current, c) => current + c);
        }
    }
}