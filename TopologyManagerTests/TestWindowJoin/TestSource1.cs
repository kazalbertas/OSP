using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSPTests.TestWindowJoin
{
    public class TestSource1 : Source<Test2>
    {
        public override DateTime ExtractEventTime(Test2 data)
        {
            return data.EventTime;
        }

        public override object GetKey(Test2 input)
        {
            return input.KeyValue;
        }

        public override TimeSpan MaxOutOfOrder()
        {
            return TimeSpan.Zero;
        }

        public override Test2 ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {
            var t = new Test2() { KeyValue = "a", ValueForAggregation = "5", EventTime = new DateTime(2019, 10, 10, 10, 10, 10) };
            Data<Test2> dt = new Data<Test2>(GetKey(t), t);
            await SendMessageToStream(dt);

            var t2 = new Test2() { KeyValue = "b", ValueForAggregation = "4", EventTime = new DateTime(2019, 10, 10, 10, 10, 11) };
            Data<Test2> dt2 = new Data<Test2>(GetKey(t2), t2);
            await SendMessageToStream(dt2);
            
            var t3 = new Test2() { KeyValue = "a", ValueForAggregation = "5", EventTime = new DateTime(2019, 10, 10, 10, 10, 12) };
            Data<Test2> dt3 = new Data<Test2>(GetKey(t3), t3);
            await SendMessageToStream(dt3);

            var t4 = new Test2() { KeyValue = "b", ValueForAggregation = "4", EventTime = new DateTime(2019, 10, 10, 10, 10, 13) };
            Data<Test2> dt4 = new Data<Test2>(GetKey(t4), t4);
            await SendMessageToStream(dt4);

            Thread.Sleep(2000);

            var t5 = new Test2() { KeyValue = "a", ValueForAggregation = "100", EventTime = new DateTime(2019, 10, 10, 10, 10, 19) };
            Data<Test2> dt5 = new Data<Test2>(GetKey(t5), t5);
            await SendMessageToStream(dt5);

        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            return new TimeSpan(0, 0, 1);
        }
    }

    public class Test2
    {
        public string KeyValue { get; set; }
        public string ValueForAggregation { get; set; }
        public DateTime EventTime { get; set; }
    }
}
