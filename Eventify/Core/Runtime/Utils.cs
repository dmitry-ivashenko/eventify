using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eventify.Core.Runtime
{
    public static class Utils
    {
        public static bool InRange(this int i, int min, int max)
        {
            return i <= max && i >= min;
        }

        public static bool InRange(this int i, float min, float max)
        {
            return i <= max && i >= min;
        }
        
        public static string RemoveFirstLines(this string text, int linesCount)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }
        
        public static bool ContainsKeys(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
        
        public static bool ContainsIgnoreCase(this string source, string toCheck, StringComparison comp = StringComparison.InvariantCultureIgnoreCase)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
        
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new ArgumentException("list length can't be zero");
            var randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }
    }
}
