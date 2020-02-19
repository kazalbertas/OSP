using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestStringHashCodePartitioner
{
    
    public class TestSink1 : Sink<string>
    {
        private string val = "";

        public override void Consume(string input)
        {
            if (val == "")
            {
                GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace).PassTest("Got a value");
                val = input;
            } 

            if (val != input)
            {
                GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace).FailTest(string.Format("Wrong value received, expected: {0}, got {1} ", val, input));
            }
        }
    }
}
