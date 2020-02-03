using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestOperators.FilterTest
{
    public class TestSink1 : Sink<string>
    {
        public override void Consume(string input)
        {
            Assert.True(input == "Test1");
        }
    }
}
