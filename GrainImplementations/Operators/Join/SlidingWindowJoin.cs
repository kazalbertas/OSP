using CoreOSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Operators.Join
{
    public abstract class SlidingWindowJoin<T, K> : SlidingWindowJoin<T, K, (T, K)>
    {
        public override (T, K) Map(T inputA, K inputB)
        {
            return (inputA, inputB);
        }
    }

    public abstract class SlidingWindowJoin<T,K,O> : WindowJoin<T,K>
    {
        public abstract TimeSpan GetWindowSize();
        public abstract TimeSpan GetSlideSize();

        public override void ProcessWindow()
        {
            if (WatermarkA > WindowStart.Add(GetWindowSize()) && WatermarkB > WindowStart.Add(GetWindowSize()))
            {
                WindowStart = WindowStart.Add(GetSlideSize());
                RemoveOutsideWindowData(WindowStart);
            }
        }

        public override async Task ProcessData(Data<K> input, Metadata metadata)
        {
            if (WindowStart == DateTime.MinValue) WindowStart = ExtractDateTime(input);
            if (WatermarkB.Subtract(AllowedLateness()) <= ExtractDateTime(input))
            {

                if (sourceBInput.ContainsKey(input.Key))
                {
                    sourceBInput[input.Key].Add(input);
                }
                else
                {
                    sourceBInput.Add(input.Key, new List<Data<K>>() { input });
                }

                //Join data
                if (ExtractDateTime(input) < WindowStart.Add(GetWindowSize()))
                {
                    if (sourceAInput.ContainsKey(input.Key))
                    {
                        var sourceAWithSameKey = sourceAInput[input.Key].Where(x => (ExtractDateTime(x) < WindowStart.Add(GetWindowSize()))).ToList();
                        foreach (var aIn in sourceAWithSameKey)
                        {
                            var dt = new Data<(T, K)>(input.Key, (aIn.Value, input.Value));
                            await SendToNextStreamData(input.Key, dt, GetMetadata());
                        }
                    }
                }
            }
        }

        public override async Task ProcessData(Data<T> input, Metadata metadata)
        {
            if (WindowStart == DateTime.MinValue) WindowStart = ExtractDateTime(input);
            if (WatermarkA.Subtract(AllowedLateness()) <= ExtractDateTime(input))
            {
                if (sourceAInput.ContainsKey(input.Key))
                {
                    sourceAInput[input.Key].Add(input);
                }
                else
                {
                    sourceAInput.Add(input.Key, new List<Data<T>>() { input });
                }
                if (ExtractDateTime(input) < WindowStart.Add(GetWindowSize()))
                {
                    if (sourceBInput.ContainsKey(input.Key))
                    {
                        var sourceBWithSameKey = sourceBInput[input.Key].Where(x => (ExtractDateTime(x) < WindowStart.Add(GetWindowSize()))).ToList();

                        foreach (var bIn in sourceBWithSameKey)
                        {
                            if (Filter(input.Value, bIn.Value))
                            {
                                var dt = new Data<O>(input.Key, Map(input.Value, bIn.Value));
                                await SendToNextStreamData(input.Key, dt, GetMetadata());
                            }
                        }
                    }
                }
            }
        }

        public virtual bool Filter(T inputA, K inputB) 
        {
            return true;
        }
        public abstract O Map(T inputA, K inputB);
    }
}
