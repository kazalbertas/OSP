using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestOperators.SourceSinkTest
{
    public class TestSink1 : Sink<string>
    {
        int index = 0;
        public override void Consume(string input)
        {
            if (index == 0)
            {
                Assert.Equal("Test1", input);
                index++;
            }
            else 
            {
                Assert.Equal("Test2", input);
            }
        }
    }
}
