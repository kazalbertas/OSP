using CoreOSP.Models;
using GrainInterfaces.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Operators
{
    public abstract class Sink<T> : Operator<T>, ISink
    {

        public abstract void Consume(T input);

        public override void ProcessCheckpoint(Checkpoint cp, Metadata metadata) {/*Do nothing right now. Checkpoint is sent to checkpoint manager to finalize snapshot.*/}

        public override void ProcessData(Data<T> input, Metadata metadata)
        {
            Consume(input.Value);
        }

        public override void ProcessWatermark(Watermark wm, Metadata metadata) {/*Do nothing*/}
    }
}
