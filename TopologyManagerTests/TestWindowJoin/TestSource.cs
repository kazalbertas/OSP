using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSPTests.TestWindowJoin
{
    public class TestSource : Source<Test>
    {
        public override DateTime ExtractTimestamp(Test data)
        {
            return data.EventTime;
        }

        public override object GetKey(Test input)
        {
            return input.KeyValue;
        }

        public override TimeSpan MaxOutOfOrder()
        {
            return TimeSpan.Zero;
        }

        public override Test ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {
            var t = new Test() { KeyValue = "a", ValueForAggregation = 5, EventTime = new DateTime(2019, 10, 10, 10, 10, 10) };
            Data<Test> dt = new Data<Test>(GetKey(t), t);
            await SendMessageToStream(dt);

            var t2 = new Test() { KeyValue = "b", ValueForAggregation = 4, EventTime = new DateTime(2019, 10, 10, 10, 10, 11) };
            Data<Test> dt2 = new Data<Test>(GetKey(t2), t2);
            await SendMessageToStream(dt2);
            
            var t3 = new Test() { KeyValue = "a", ValueForAggregation = 5, EventTime = new DateTime(2019, 10, 10, 10, 10, 12) };
            Data<Test> dt3 = new Data<Test>(GetKey(t3), t3);
            await SendMessageToStream(dt3);

            var t4 = new Test() { KeyValue = "b", ValueForAggregation = 4, EventTime = new DateTime(2019, 10, 10, 10, 10, 13) };
            Data<Test> dt4 = new Data<Test>(GetKey(t4), t4);
            await SendMessageToStream(dt4);

            Thread.Sleep(2000);

            var t5 = new Test() { KeyValue = "a", ValueForAggregation = 100, EventTime = new DateTime(2019, 10, 10, 10, 10, 19) };
            Data<Test> dt5 = new Data<Test>(GetKey(t5), t5);
            await SendMessageToStream(dt5);

        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            return new TimeSpan(0, 0, 1);
        }
    }

    public class Test
    {
        public string KeyValue { get; set; }
        public int ValueForAggregation { get; set; }
        public DateTime EventTime { get; set; }
    }
}
