using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSPTests.TestEventBasedWindow
{
    public class TestSource1 : Source<TerminationEvent>
    {
        public override DateTime ExtractTimestamp(TerminationEvent data)
        {
            throw new NotImplementedException();
        }

        public override object GetKey(TerminationEvent input)
        {
            return input.Key;
        }

        public override TimeSpan MaxOutOfOrder()
        {
            throw new NotImplementedException();
        }

        public override TerminationEvent ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {

            Thread.Sleep(2000);

            var t5 = new TerminationEvent() { Key = "a" };
            SendToNextStreamData(GetKey(t5), t5, GetMetadata());

        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            return new TimeSpan(0, 0, 1);
        }
    }
}
