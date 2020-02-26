﻿using CoreOSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Operators.Join
{
    public abstract class SlidingWindowJoin<T,K> : WindowJoin<T,K>
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
                sourceBInput.Add(input);
                //Join data
                if (ExtractDateTime(input) < WindowStart.Add(GetWindowSize()))
                {
                    var sourceAWithSameKey = sourceAInput.Where(x => x.Key.Equals(input.Key) && (ExtractDateTime(x) < WindowStart.Add(GetWindowSize()))).ToList();
                    foreach (var aIn in sourceAWithSameKey)
                    {
                        var dt = new Data<(T, K)>(input.Key, (aIn.Value, input.Value));
                        await SendToNextStreamData(input.Key, dt, GetMetadata());
                    }
                }
            }
        }

        public override async Task ProcessData(Data<T> input, Metadata metadata)
        {
            if (WindowStart == DateTime.MinValue) WindowStart = ExtractDateTime(input);
            if (WatermarkA.Subtract(AllowedLateness()) <= ExtractDateTime(input))
            {
                sourceAInput.Add(input);
                if (ExtractDateTime(input) < WindowStart.Add(GetWindowSize()))
                {
                    var sourceBWithSameKey = sourceBInput.Where(x => x.Key.Equals(input.Key) && (ExtractDateTime(x) < WindowStart.Add(GetWindowSize()))).ToList();

                    foreach (var bIn in sourceBWithSameKey)
                    {
                        var dt = new Data<(T, K)>(input.Key, (input.Value, bIn.Value));
                        await SendToNextStreamData(input.Key, dt, GetMetadata());
                    }
                }
            }
        }
    }
}
