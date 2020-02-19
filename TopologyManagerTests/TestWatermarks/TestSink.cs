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
        public override void ProcessWatermark(Watermark wm, Metadata metadata)
        {
            wmCount++;
            if (wmCount != 2)
            {
                StaticTestHelper.TempFailTest("Wmcount !=2 actual: " + wmCount);
            }
            else 
            {
                StaticTestHelper.PassTest("Wmcount == 2");
            }
        }

        public override void Consume(string input)
        {

        }
    }
}
