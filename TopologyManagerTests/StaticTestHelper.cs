using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTests
{
    public static class StaticTestHelper
    {
        public static bool Failed { get; set; } = false;

        public static bool TempFailed { get; set; } = false;

        public static List<string> Messages { get; set; } = new List<string>();

        public static void TempFailTest(string message)
        {
            Messages.Add(message);
            TempFailed = true;
        }

        public static void FailTest(string message)
        {
            Messages.Add(message);
            Failed = true;
        }

        public static (bool, string) GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var m in Messages)
            {
                sb.AppendLine(m);
            }
            var result = sb.ToString();
            sb.Clear();
            Messages.Clear();
            return (!(Failed == false && TempFailed == false), result);
        }

        public static void PassTest(string message)
        {
            Messages.Add(message);
            TempFailed = false;
        }

        public static void Reset()
        {
            Failed = false;
            Messages.Clear();
            TempFailed = false;
        }

        public static void LogMessage(string message)
        {
            Messages.Add(message);
        }
    }
}
