using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSPTests.TestWatermarks
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

        public override async Task Start()
        {
            Data<string> dt = new Data<string>(GetKey("TestKey"), "Test1");
            await SendMessageToStream(dt);

            Data<string> dt2 = new Data<string>(GetKey("TestKey"), "Test1");
            await SendMessageToStream(dt2);

            Thread.Sleep(5000);

            Data<string> dt3 = new Data<string>(GetKey("TestKey"), "Test1");
            await SendMessageToStream(dt3);

            Data<string> dt4 = new Data<string>(GetKey("TestKey"), "Test1");
            await SendMessageToStream(dt4);
        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            return new TimeSpan(0,0,5);
        }
    }
}
