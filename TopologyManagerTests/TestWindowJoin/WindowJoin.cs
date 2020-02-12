using CoreOSP.Models;
using GrainImplementations.Operators.Join;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTests.TestWindowJoin
{
    public class WindowJoin : TumblingWindowJoin<Test, Test2>
    {
        public override DateTime ExtractTimestamp(Data<Test> input)
        {
            return input.Value.EventTime;
        }

        public override DateTime ExtractTimestamp(Data<Test2> input)
        {
            return input.Value.EventTime;
        }

        public override TimeSpan GetWindowSize()
        {
            return new TimeSpan(0, 0, 5);
        }
    }
}
