using System;
using System.Collections.Generic;
using System.Text;

namespace GrainImplementations.Operators.Aggregation
{
    public abstract class TumblingWindowAggregation<T,K> : SlidingWindowAggregation<T,K>
    {
        public override TimeSpan GetSlideSize()
        {
            return GetWindowSize();
        }
    }
}
