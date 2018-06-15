using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DocGen.Core
{
    public static class StringExtensions
    {
        public static string NormalizeLineEndings(this string value)
        {
            return value.ReplaceLineEndings(Environment.NewLine);
            /*if (string.IsNullOrEmpty(value)) return value;
            var result = new StringBuilder();
            using (var sr = new StringReader(value))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    result.AppendLine(line);
                }
            }

            return result.ToString();*/
        }
        
        
        
        public static string ReplaceLineEndings(this string value, string lineEndings)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return Regex.Replace(value, @"(\r\n)|(\n\r)|(\n\r)|(\r)", lineEndings);
        }
    }
}