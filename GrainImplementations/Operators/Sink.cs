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

        public override Task ProcessCheckpoint(Checkpoint cp, Metadata metadata) 
        {
            /*Do nothing right now. Checkpoint is sent to checkpoint manager to finalize snapshot.*/return Task.CompletedTask; 
        }

        public override Task ProcessData(Data<T> input, Metadata metadata)
        {
            Consume(input.Value);
            return Task.CompletedTask;
        }

        public override Task ProcessWatermark(Watermark wm, Metadata metadata) 
        {
            /*Do nothing*/ 
            return Task.CompletedTask; 
        }
    }
}
