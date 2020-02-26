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

        public override Task ProcessCheckpoint(Checkpoint cp, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public override async Task ProcessData(Data<T> input, Metadata metadata)
        {
            if (Apply(input.Value)) 
            {
                input.ProcessingTime = DateTime.Now;

                await SendToNextStreamData(input.Key, input, GetMetadata());
            }
        }

        public override Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            throw new NotImplementedException();
        }

    }
}
