using CoreOSP.Models;
using GrainInterfaces.Operators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrainImplementations.Operators.Aggregation
{
    //Continuous event based aggregation
    public abstract class EventBasedAggregation<T, K> : Operator<T>, IEventBasedAggregation
    {

        private List<Data<T>> data = new List<Data<T>>();

        //returns true if update
        public abstract bool CheckUpdateEvent(T input, T aggEvent);

        public override async Task ProcessData(Data<T> input, Metadata metadata)
        {
            data.RemoveAll(x => x.Key.Equals(input.Key) && CheckUpdateEvent(input.Value, x.Value));
            data.Add(input);
            var filteredData = data.FindAll(x => x.Key.Equals(input.Key));
            var result = AggregateResults(filteredData.Select(x=>x.Value).ToList());
            if (Filter(result)) 
            {
                await SendToNextStreamData(input.Key, new Data<K>(input.Key, result), GetMetadata());
            }
        }

        public override Task ProcessTerminationEvent(Data<TerminationEvent> tevent)
        {
            data.RemoveAll(x => x.Key.Equals(tevent.Value.Key));
            return Task.CompletedTask;
            // check direct injection or pass.
        }

        public abstract K AggregateResults(List<T> items);

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
