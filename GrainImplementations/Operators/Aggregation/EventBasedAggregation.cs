using CoreOSP.Models;
using GrainInterfaces.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrainImplementations.Operators.Aggregation
{
    //Continuous event based aggregation
    public abstract class EventBasedAggregation<T, K> : Operator<T>, IEventBasedAggregation
    {

        private Dictionary<object,List<Data<T>>> data = new Dictionary<object, List<Data<T>>>();

        private DateTime Watermark { get; set; }
        private List<TerminationEvent> TerminationEvent { get;} = new List<TerminationEvent>();

        //returns true if update
        public abstract bool CheckUpdateEvent(T input, T aggEvent);

        public override async Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            Watermark = wm.TimeStamp;
            foreach (var e in TerminationEvent.ToArray())
            {
                if (e.TimeStamp <= Watermark) 
                {
                    RemoveProcessed(e.Key, e.TimeStamp);
                    TerminationEvent.Remove(e);
                }
            }
            await SendToNextStreamWatermark(wm, GetMetadata());
        }

        public override async Task ProcessData(Data<T> input, Metadata metadata)
        {
            if (data.ContainsKey(input.Key))
            {
                data[input.Key].RemoveAll(x => CheckUpdateEvent(input.Value, x.Value));
                data[input.Key].Add(input);
            }
            else 
            {
                data.Add(input.Key, new List<Data<T>>() { input });
            }
            var filteredData = data[input.Key];
            var result = AggregateResults(filteredData.Select(x=>x.Value).ToList());
            if (Filter(result)) 
            {
                await SendToNextStreamData(input.Key, new Data<K>(input.Key, result), GetMetadata());
            }
        }

        private void RemoveProcessed(object key, DateTime timestamp) 
        {
            if (data.ContainsKey(key))
            {
                data[key].RemoveAll(x => ExtractDateTime(x.Value) <= timestamp);
                if (data[key].Count == 0)
                {
                    data.Remove(key);
                }
            }
            
        }

        public override Task ProcessTerminationEvent(Data<TerminationEvent> tevent)
        {
            TerminationEvent.Add(tevent.Value);
            foreach (var e in TerminationEvent.ToArray())
            {
                if (e.TimeStamp <= Watermark)
                {
                    RemoveProcessed(e.Key, e.TimeStamp);
                    TerminationEvent.Remove(e);
                }
            }
            return Task.CompletedTask;
        }

        public abstract K AggregateResults(List<T> items);
        public abstract DateTime ExtractDateTime(T item);
        // figure out change of type for map
        public virtual K Map(K input) 
        {
            return input;
        }

        public virtual bool Filter(K input)
        {
            return true;
        }
    }
}
