using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestWindowAggregation
{
    
    public class TestSink : Sink<int>
    {

        public override void Consume(int input)
        {
            StaticTestHelper.LogMessage("Logging input: " + input);
            if (input == 8 || input == 10)
            {
                StaticTestHelper.PassTest("Got expected value");
            }
            else 
            {
                StaticTestHelper.FailTest("got unexpected value : " + input);
            }
        }
    }
}
