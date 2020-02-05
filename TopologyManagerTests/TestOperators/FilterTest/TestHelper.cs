using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSPTests.TestOperators.FilterTest
{
    public interface ITestHelper : IGrainWithIntegerKey
    {
        Task ShouldBreak();
        Task<bool> GetBreaking();
        Task Reset();
    }
    public class TestHelper : Grain, ITestHelper
    {
        public bool Breaking { get; set; } = false;

        public Task ShouldBreak() { Breaking = true; return Task.CompletedTask; }
        public Task<bool> GetBreaking() { return Task.FromResult(Breaking); }

        public Task Reset()
        {
            Breaking = false;
            return Task.CompletedTask;
        }
    }
}
