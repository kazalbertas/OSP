using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSPTests.TestStringHashCodePartitioner
{
    public class TestSource : Source<string>
    {
        public override DateTime ExtractEventTime(string data)
        {
            throw new NotImplementedException();
        }

        public override object GetKey(string input)
        {
            return input;
        }

        public override TimeSpan MaxOutOfOrder()
        {
            throw new NotImplementedException();
        }

        public override string ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        //TODO change to SendMessage when watermarks will be able to be disabled.
        public override async Task Start()
        {
            Data<string> dt = new Data<string>(GetKey("abc"), "Test2");
            await SendToNextStreamData(dt.Key, dt, GetMetadata());

            Data<string> dt2 = new Data<string>(GetKey("def"), "Test1");
            await SendToNextStreamData(dt2.Key, dt2, GetMetadata());

            Data<string> dt3 = new Data<string>(GetKey("abc"), "Test2");
            await SendToNextStreamData(dt3.Key, dt3, GetMetadata());

            Data<string> dt4 = new Data<string>(GetKey("def"), "Test1");
            await SendToNextStreamData(dt4.Key, dt4, GetMetadata());
        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            throw new NotImplementedException();
        }
    }
}
