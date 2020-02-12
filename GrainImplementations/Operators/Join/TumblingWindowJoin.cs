using System;
using System.Collections.Generic;
using System.Text;

namespace GrainImplementations.Operators.Join
{
    public abstract class TumblingWindowJoin<T,K> : SlidingWindowJoin<T,K>
    {
        public override TimeSpan GetSlideSize()
        {
            return GetWindowSize();
        }
    }
}
