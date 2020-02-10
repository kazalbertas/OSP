using CoreOSP.Models;
using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestWatermarks
{
    
    public class TestSink : Sink<string>
    {
        public int wmCount = 0;
        public override void ProcessWatermark(Watermark wm)
        {
            wmCount++;
            if (wmCount != 2)
            {
                GrainFactory.GetGrain<ITestHelper>(0).ShouldBreak();
            }
            else 
            {
                GrainFactory.GetGrain<ITestHelper>(0).Reset();
            }
        }

        public override void Consume(string input)
        {

        }
    }
}
