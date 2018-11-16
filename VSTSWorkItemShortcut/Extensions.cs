using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace VSTSWorkItemShortcut
{
    public static class Extensions
    {
        public static bool Between(this float input, int val1, int val2)
        {
            return (input >= val1 && input <= val2) ? true : false;
        }

        public static bool IsContainsAlphabets(this string input) => Regex.Matches(input, @"[a-zA-Z]").Count > 0;
    }
}
