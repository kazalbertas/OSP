using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace GrainImplementations.Operators.Aggregation
{
    /// <summary>
    /// Key window aggregation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WindowAggregation<T, K> : Operator<T>, IWindowAggregation
    {
        private List<Data<T>> data = new List<Data<T>>();

        public DateTime WindowStart = DateTime.MinValue;
        public DateTime Watermark = DateTime.MinValue;

        public override Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            Watermark = wm.TimeStamp;
            if (WindowStart != DateTime.MinValue) ProcessWindow(data, Watermark);
            return Task.CompletedTask;
        }

        public override Task ProcessData(Data<T> input, Metadata metadata)
        {
            if (WindowStart == DateTime.MinValue) WindowStart = ExtractTimestamp(input);
            if (Watermark.Subtract(AllowedLateness()) <= ExtractTimestamp(input)) data.Add(input);
            return Task.CompletedTask;
        }

        internal void RemoveOutsideWindowData(DateTime newWindowStart) 
        {
            data = data.Where(x => ExtractTimestamp(x) >= newWindowStart).ToList();
        }

        public abstract K Aggregate(List<T> inputs);

        public abstract void ProcessWindow(List<Data<T>> inputs, DateTime wm);

        public virtual TimeSpan AllowedLateness()
        {
            return TimeSpan.Zero;
        }

        public abstract DateTime ExtractTimestamp(Data<T> input);
    }
}
