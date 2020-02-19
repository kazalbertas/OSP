using CoreOSP.Models;
using GrainImplementations.Operators;
using GrainInterfaces.Operators;
using Orleans;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace OSPTests.TestOperators.MapTest
{
    public class TestSource1 : Source<TypeA>
    {
        public override DateTime ExtractTimestamp(TypeA data)
        {
            throw new NotImplementedException();
        }

        public override object GetKey(TypeA input)
        {
            return input.Field1;
        }

        public override TimeSpan MaxOutOfOrder()
        {
            return new TimeSpan(0, 0, 1);
        }

        public override TypeA ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {
           


            TypeA a1 = new TypeA() { Field1 = "a",Field2 = 0, Field3 = "b"};
            TypeA a2 = new TypeA() { Field1 = "a", Field2 = 1, Field3 = "b" };

            var dt = new Data<TypeA>(GetKey(a1), a1);
            var dt2 = new Data<TypeA>(GetKey(a2), a2);
            SendMessageToStream(dt);
            SendMessageToStream(dt2);
            //SendToNextStreamData(dt.Key, dt, GetMetadata());
            //SendToNextStreamData(dt2.Key, dt2, GetMetadata());
            //(var NextOperatorId, var NextOperatorClass) = _delegator.DelegateToProcess(dt.Key);
            //var grain = GrainFactory.GetGrain<IOperator>(NextOperatorId, NextOperatorClass.FullName);
            //await grain.Process(dt, GetMetadata());
            //await grain.Process(dt2, GetMetadata());

        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            return new TimeSpan(0, 0, 1);
        }
    }
}
