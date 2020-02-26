using CoreOSP.Models;
using GrainImplementations.Operators.Join;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTests.TestWindowJoin
{
    public class WindowJoin : TumblingWindowJoin<Test, Test2>
    {
        public override DateTime ExtractEventTime(Test input)
        {
            return input.EventTime;
        }

        public override DateTime ExtractEventTime(Test2 input)
        {
            return input.EventTime;
        }

        public override TimeSpan GetWindowSize()
        {
            return new TimeSpan(0, 0, 5);
        }
    }
}
