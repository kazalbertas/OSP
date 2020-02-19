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

        public override void ProcessCheckpoint(Checkpoint cp, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public override void ProcessData(Data<T> input, Metadata metadata)
        {
            if (Apply(input.Value)) 
            {
                input.TimeStamp = DateTime.Now;

                SendToNextStreamData(input.Key, input, GetMetadata());
            }
        }

        public override void ProcessWatermark(Watermark wm, Metadata metadata)
        {
            throw new NotImplementedException();
        }

    }
}
