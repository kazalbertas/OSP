using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestWindowJoin
{
    
    public class TestSink : Sink<(Test,Test2)>
    {
        bool failed = false;

        public override void Consume((Test, Test2) input)
        {
            (var in1, var in2) = input;

            if ((in1.ValueForAggregation == 5 || in1.ValueForAggregation == 4) &&
                (in2.ValueForAggregation == "5" || in2.ValueForAggregation == "4") &&
                !failed)
            {
                GrainFactory.GetGrain<ITestHelper>(0).Reset();
            }
            else
            {
                failed = true;
                GrainFactory.GetGrain<ITestHelper>(0).ShouldBreak();
            }
        }
    }
}
