using CoreOSP.Models;
using GrainImplementations.Operators;
using GrainInterfaces.Operators;
using Orleans;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace OSPTests.TestOperators.FilterTest
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
            Data<string> dt = new Data<string>(GetKey("TestKey"), "Test2");
            //(var NextOperatorId, var NextOperatorClass) = _delegator.DelegateToProcess(dt.Key);
            // var grain = GrainFactory.GetGrain<IOperator>(NextOperatorId, NextOperatorClass.FullName);
            // await grain.Process(dt, GetMetadata());
            SendToNextStreamData(dt.Key, dt, GetMetadata());
            Data<string> dt2 = new Data<string>(GetKey("TestKey"), "Test1");
            //await grain.Process(dt2, GetMetadata());
            SendToNextStreamData(dt2.Key, dt2, GetMetadata());
            Data<string> dt3 = new Data<string>(GetKey("TestKey"), "Test2");
            //await grain.Process(dt3, GetMetadata());
            SendToNextStreamData(dt3.Key, dt3, GetMetadata());
            Data<string> dt4 = new Data<string>(GetKey("TestKey"), "Test1");
            SendToNextStreamData(dt4.Key, dt4, GetMetadata());
            //await grain.Process(dt4, GetMetadata());
            //return Task.CompletedTask;
        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            throw new NotImplementedException();
        }
    }
}
