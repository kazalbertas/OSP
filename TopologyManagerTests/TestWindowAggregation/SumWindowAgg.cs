using CoreOSP.Models;
using GrainImplementations.Operators.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
