using CoreOSP.Models;
using GrainImplementations.Operators;
using GrainInterfaces.Operators;
using Orleans;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace OSPTests.TestOperators.SourceSinkTest
{
    public class TestSource1 : Source<string>
    {
        public override DateTime ExtractTimestamp(string data)
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
            await SendToNextStreamData(dt.Key, dt, GetMetadata());
            Data<string> dt2 = new Data<string>(GetKey("TestKey"), "Test2");
            await SendToNextStreamData(dt2.Key, dt2, GetMetadata());
        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            throw new NotImplementedException();
        }
    }
}
