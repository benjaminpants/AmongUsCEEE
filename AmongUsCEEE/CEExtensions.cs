// AmongUsCEDE.Extensions.CEExtensions
using System;
using System.IO;
using UnityEngine;

namespace AmongUsCEEE
{
    public static class CEExtensions
    {
        public static string GetGameDirectory()
        {
            return Directory.GetParent(Application.dataPath).FullName;
        }

        private static bool IsCharacterAVowel(char c)
        {
            string vowels = "aeiou";
            return vowels.IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public static string AOrAn(this string input, bool firstlettercap)
        {
            return (firstlettercap ? "A" : "a") + (IsCharacterAVowel(input[0]) ? "n" : "");
        }
    }
}
