using CoreOSP.Models;
using GrainImplementations.Operators.Aggregation;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSPTests.TestWindowAggregation
{
    public class Test 
    {
        public string KeyValue { get; set; }
        public int ValueForAggregation { get; set; }
        public DateTime EventTime { get; set; }
    }

    public class SumWindowAgg : TumblingWindowAggregation<Test,int>
    {
        public override Task ProcessData(Data<Test> input, Metadata metadata)
        {
            StaticTestHelper.LogMessage("Window start: " + this.WindowStart);
            StaticTestHelper.LogMessage("Logginge event time: " + input.Value.EventTime);
            base.ProcessData(input, metadata);
            return Task.CompletedTask;
        }

        public override Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            StaticTestHelper.LogMessage("Watermarktime: " + wm.TimeStamp);
            base.ProcessWatermark(wm, metadata);
            return Task.CompletedTask;
        }

        public override int Aggregate(List<Test> inputs)
        {
            return inputs.Select(x => x.ValueForAggregation).Sum();
        }

        public override DateTime ExtractTimestamp(Data<Test> input)
        {
            
            return input.Value.EventTime;
        }

        public override TimeSpan GetWindowSize()
        {
            return new TimeSpan(0, 0, 5);
        }
    }
}
