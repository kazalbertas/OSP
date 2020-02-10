using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSPTests.TestWatermarks
{
    public class TestObject 
    {
        public string SomeData { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class TestSource1 : Source<TestObject>
    {
        public override DateTime ExtractTimestamp(TestObject data)
        {
            return data.TimeStamp;
        }

        public override object GetKey(TestObject input)
        {
            return input.SomeData;
        }

        public override TimeSpan MaxOutOfOrder()
        {
            return new TimeSpan(0, 0, 1);
        }

        public override TestObject ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public async override Task Start()
        {
            TestObject dtt = new TestObject() { SomeData = "a", TimeStamp = new DateTime(2019,10,10,10,10,10) };
            Data<TestObject> dt = new Data<TestObject>(GetKey(dtt),dtt);
            SendMessageToStream(dt);
            Thread.Sleep(1500);
            TestObject dtt2 = new TestObject() { SomeData = "b", TimeStamp = new DateTime(2019, 10, 10, 10, 10, 10, 100) };
            Data<TestObject> dt2 = new Data<TestObject>(GetKey(dtt2), dtt2);
            SendMessageToStream(dt2);
            Thread.Sleep(1500);
            TestObject dtt3 = new TestObject() { SomeData = "c", TimeStamp = new DateTime(2019, 10, 10, 10, 10, 11, 500) };
            Data<TestObject> dt3 = new Data<TestObject>(GetKey(dtt3), dtt3);
            SendMessageToStream(dt3);
        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            return new TimeSpan(0, 0, 1);
        }
    }
}
