using CoreOSP.Models;
using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OSPTests.TestWatermarks
{
    
    public class TestSink1 : Sink<TestObject>
    {
        public int wmCount = 0;
        public bool error = false;
        public override Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            if (wmCount == 0)
            {
                if (wm.TimeStamp != new DateTime(2019, 10, 10, 10, 10, 9)) error = true;
            }
            else if (wmCount == 1)
            {
                if (wm.TimeStamp != new DateTime(2019, 10, 10, 10, 10, 10, 100)) error = true;
            }
            else if (wmCount == 2)
            {
                if (wm.TimeStamp != new DateTime(2019, 10, 10, 10, 10, 11, 500)) error = true;
            }

                wmCount++;
            if (wmCount != 3)
            {
                StaticTestHelper.TempFailTest("Wmcount != 3, actual: " + wmCount);
            }
            else 
            {
                if (!error) StaticTestHelper.PassTest("WmCount == 3");
            }
            return Task.CompletedTask;
        }


        public override void Consume(TestObject input)
        {

        }
    }
}
