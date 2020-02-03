using CoreOSP.Models;
using GrainInterfaces.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Operators
{
    public abstract class Filter<T> : Operator<T>, IFilter
    {
        public abstract bool Apply(T input);

        public override void ProcessCheckpoint(Checkpoint cp)
        {
            throw new NotImplementedException();
        }

        public override void ProcessData(Data<T> input, Metadata metadata)
        {
            if (Apply(input.Value)) 
            {
                var nextOp = _partitioner.GetNextStream(input.Key);
                input.TimeStamp = DateTime.Now;
                //GrainFactory.GetGrain<IOperator>(nextOp.Item1,nextOp.Item2.FullName).Process(input, GetMetadata());
                SendToNextStreamAsync(input.Key, input, GetMetadata());
            }
        }

        public override void ProcessWatermark(Watermark wm)
        {
            throw new NotImplementedException();
        }

    }
}
