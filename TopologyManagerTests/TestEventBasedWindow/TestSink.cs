using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestEventBasedWindow
{
    
    public class TestSink : Sink<int>
    {
        bool failed = false;
        int count = 0;
        public override void Consume(int input)
        {

            if (input == 4 || input == 5 || input == 100 ||input == 8 || input == 10 && !failed)
            {
                GrainFactory.GetGrain<ITestHelper>(5).Reset();
                count++;
            }
            else 
            {
                failed = true;
                GrainFactory.GetGrain<ITestHelper>(5).ShouldBreak();
            }

            if (count != 5) 
            {
                GrainFactory.GetGrain<ITestHelper>(5).ShouldBreak();
            }
        }
    }
}
