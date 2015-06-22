using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace cs2ts
{
    public class BaseTest
    {
        protected static Visitor Parse(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var visitor = new Visitor();
            visitor.Visit(root);
            return visitor;
        }

        private static string DisplayText(string text)
        {
            var dot = '·';
            var cr = '¶';
            var lf = '§';

            return text.Replace(' ', dot).Replace('\n', cr).Replace('\r', lf);
        }

        private static string DisplayLists(string[] expected, string[] actual)
        {
            const int offset = 5;
            int width = 95;

            width = 1 + Math.Max(expected.Max(x => x.Length), actual.Max(x => x.Length));

            string LineFormat = string.Format("{{0,-{0}}} | {{1,-{0}}}\n", width);

            StringBuilder result = new StringBuilder("\n\n  ");
            result.AppendFormat(LineFormat, "Expected", "Actual");
            result.AppendLine(new string('-', width * 2 + offset));

            for (int index = 0; index < Math.Max(expected.Length, actual.Length); index++)
            {
                string expectedLine = GetLine(index, expected);
                string actualLine = GetLine(index, actual);
                string flag = expectedLine == actualLine ? " " : new string(Convert.ToChar(187), 1);
                result.Append(flag + " ");
                result.AppendFormat(LineFormat, DisplayText(expectedLine), DisplayText(actualLine));
            }

            return result.ToString();
        }

        private static string GetLine(int index, string[] lines)
        {
            return index < lines.Length ? lines[index] : "";
        }

        protected static void Compare(string code, string[] expected)
        {
            var actual = Parse(code).Output();
            var actualAsList = actual.Replace("\r", "").Split(new string[] { "\n" }, StringSplitOptions.None);

            var dump = DisplayLists(expected, actualAsList);
            try
            {
                CollectionAssert.AreEqual(expected, actualAsList, dump);
            }
            catch (AssertionException)
            {
                Debug.WriteLine(dump);
                throw;
            }
        }
    }
}
