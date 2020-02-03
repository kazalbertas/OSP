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
        public override object GetKey(string input)
        {
            return input;
        }



        public override string ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {
            Data<string> dt = new Data<string>(GetKey("TestKey"), "Test1");
            //(var NextOperatorId, var NextOperatorClass) = _partitioner.GetNextStream(dt.Key);
            //var grain = GrainFactory.GetGrain<IOperator>(NextOperatorId, NextOperatorClass.FullName);
            SendToNextStreamAsync(dt.Key, dt, GetMetadata());
            //await grain.Process(dt, GetMetadata());
            Data<string> dt2 = new Data<string>(GetKey("TestKey"), "Test2");
            //await grain.Process(dt2, GetMetadata());
            SendToNextStreamAsync(dt2.Key, dt2, GetMetadata());
            //return Task.CompletedTask;
        }
    }
}
