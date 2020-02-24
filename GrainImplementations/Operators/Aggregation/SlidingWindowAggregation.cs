using CoreOSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrainImplementations.Operators.Aggregation
{
    public abstract class SlidingWindowAggregation<T,K> : WindowAggregation<T,K>
    {

        public override async void ProcessWindow(List<Data<T>> inputs, DateTime watermark)
        {
            if (watermark > WindowStart.Add(GetWindowSize()))
            {
                var groups = inputs.Where(x => ExtractTimestamp(x) < WindowStart.Add(GetWindowSize())).GroupBy(x=>x.Key);
                foreach (var group in groups)
                {
                    var content = group.Select(x => x.Value).ToList();
                    await SendToNextStreamData(group.Key, new Data<K>(group.Key,Aggregate(content)), GetMetadata());
                }
                WindowStart = WindowStart.Add(GetSlideSize());
                RemoveOutsideWindowData(WindowStart);
            }
        }

        public abstract TimeSpan GetWindowSize();
        public abstract TimeSpan GetSlideSize();
    }
}
