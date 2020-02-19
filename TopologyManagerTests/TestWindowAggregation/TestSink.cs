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

            if (input == 8 || input == 10)
            {
                GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace).PassTest("Got expected value");
            }
            else 
            {
                GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace).FailTest("got unexpected value : " + input);
            }
        }
    }
}
