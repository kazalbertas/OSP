using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSPTests
{
    public interface ITestHelper : IGrainWithStringKey
    {
        Task TempFailTest(string message);
        Task FailTest(string message);
        Task<(bool,string)> GetStatus();
        Task PassTest(string message);
        Task LogMessage(string message);
        Task Reset();
    }
    public class TestHelper : Grain, ITestHelper
    {
        public bool Failed { get; set; } = false;

        public bool TempFailed { get; set; } = false;

        public List<string> Messages { get; set; } = new List<string>();

        public Task TempFailTest(string message)
        {
            Messages.Add(message);
            TempFailed = true;
            return Task.CompletedTask;
        }

        public Task FailTest(string message)
        {
            Messages.Add(message);
            Failed = true;
            return Task.CompletedTask;
        }

        public Task<(bool, string)> GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var m in Messages) 
            {
                sb.AppendLine(m);
            }
            var result = sb.ToString();
            sb.Clear();
            return Task.FromResult((!(Failed == false && TempFailed == false), result));
        }

        public Task PassTest(string message)
        {
            Messages.Add(message);
            TempFailed = false;
            return Task.CompletedTask;
        }

        public Task Reset()
        {
            Failed = false;
            TempFailed = false;
            Messages.Clear();
            return Task.CompletedTask;
        }

        public Task LogMessage(string message)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
