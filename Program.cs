using System;
using System.Text;
using System.Windows;
using static System.Console;

namespace LogInjector
{
    static class Program
    {
        public static string OriginalText { get; set; } = Clipboard.GetText();

        public static bool FirstRun { get; set; } = true;

        [STAThread]
        static void Main(string[] args)
        {
            while (true)
            {
                WriteLine($"{NewLineDeterminant()}" +
                          "LogInjector:\n" +
                                  "1. Intersect all copied lines with logs and copy to the clipboard.\n" +
                                  "2. Copy original text back to clipboard\n" +
                                  "3. Exit");

                switch (ReadKey(true).KeyChar)
                {
                    case '1':
                        WriteLine("Enter the text that will appear before counter. I.E ''MyLogMethod('' 2)");
                        var preText = ReadLine();

                        WriteLine("Enter the text that will appear after the counter. I.E MyLogMethod(2'', withJelly)''");
                        var postText = ReadLine();

                        InjectLogs(preText, postText);

                        Clear();
                        WriteLine("Log text copied to clipboard");
                        break;

                    case '2':
                        Clipboard.SetText(OriginalText);

                        Clear();
                        WriteLine("Original text returned to clipboard.");
                        break;

                    case '3':
                        return;

                    default:
                        Clear();
                        WriteLine("Invalid input.");
                        break;
                }
            }
        }

        [STAThread]
        public static void InjectLogs(string preLogText, string postLogText)
        {
            OriginalText = Clipboard.GetText();

            var lines = OriginalText.Split('\n');

            var sb = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                if(lines[i].ShouldSkip()) continue;

                sb.Append($"{preLogText}{i}: {lines[i].Trim()}{postLogText}");
                sb.Append(Environment.NewLine);
                sb.Append(lines[i]);
            }

            Clipboard.SetText(sb.ToString());
        }

        private static bool ShouldSkip(this string entry)
        {
            entry = entry.Trim();

            return entry is "}" || entry is "{" || entry.Length is 0;
        }

        private static string NewLineDeterminant()
        {
            if (!FirstRun) return $"{Environment.NewLine}{Environment.NewLine}";

            FirstRun = false;

            return "";
        }
    }
}