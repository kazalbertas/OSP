using GrainImplementations.Operators.Aggregation;
using GrainInterfaces.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSPTests.TestEventBasedWindow
{
    public class TestEventBasedAggregationM : EventBasedAggregation<Test, int>
    {
        public override int AggregateResults(List<Test> items)
        {
            return items.Sum(x => x.ValueForAggregation);
        }

        public override bool CheckUpdateEvent(Test input, Test aggEvent)
        {
            //return input.Id == aggEvent.Id;
            return false;
        }

        public override DateTime ExtractDateTime(Test item)
        {
            return item.EventTime;
        }
    }
}
