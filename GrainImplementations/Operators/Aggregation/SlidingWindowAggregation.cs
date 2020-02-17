using CoreOSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrainImplementations.Operators.Aggregation
{
    public abstract class SlidingWindowAggregation<T,K> : WindowAggregation<T,K>
    {

        public override void ProcessWindow(List<Data<T>> inputs, DateTime watermark)
        {
            if (watermark > WindowStart.Add(GetWindowSize()))
            {
                var groups = inputs.Where(x => ExtractTimestamp(x) < WindowStart.Add(GetWindowSize())).GroupBy(x=>x.Key);
                foreach (var group in groups)
                {
                    var content = group.Select(x => x.Value).ToList();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    SendToNextStreamData(group.Key, new Data<K>(group.Key,Aggregate(content)), GetMetadata());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                }
                WindowStart = WindowStart.Add(GetSlideSize());
                RemoveOutsideWindowData(WindowStart);
            }
        }

        public abstract TimeSpan GetWindowSize();
        public abstract TimeSpan GetSlideSize();
    }
}
