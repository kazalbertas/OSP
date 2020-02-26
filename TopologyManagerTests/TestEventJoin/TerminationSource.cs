using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSPTests.TestEventJoin
{
    public class TerminationSource : Source<TerminationEvent>
    {
        public override DateTime ExtractEventTime(TerminationEvent data)
        {
            return data.TimeStamp;
        }

        public override object GetKey(TerminationEvent input)
        {
            return input.Key;
        }

        public override TimeSpan MaxOutOfOrder()
        {
            return TimeSpan.Zero;
        }

        public override TerminationEvent ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {
            //Thread.Sleep(2000);

            var t5 = new TerminationEvent() { Key = "a" , TimeStamp = new DateTime(2019, 10, 10, 10, 10, 13) };
            var dt5 = new Data<TerminationEvent>(GetKey(t5), t5);
            await SendMessageToStream(dt5);
        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            return new TimeSpan(0, 0, 1);
        }
    }
}
