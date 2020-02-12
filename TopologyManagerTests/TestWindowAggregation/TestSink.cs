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
        bool failed = false;

        public override void Consume(int input)
        {

            if (input == 8 || input == 10 && !failed)
            {
                GrainFactory.GetGrain<ITestHelper>(5).Reset();
            }
            else 
            {
                failed = true;
                GrainFactory.GetGrain<ITestHelper>(5).ShouldBreak();
            }
        }
    }
}
