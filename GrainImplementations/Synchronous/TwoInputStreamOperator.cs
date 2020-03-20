using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans;
using Orleans.Streams;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Synchronous
{
    /// <summary>
    /// For Joins/Unions
    /// </summary>
    public abstract class TwoInputStreamOperator<In1,In2,Out> : Operator, ITwoInputStreamsOperator
    {
        private List<Guid> SourceA;
        private List<Guid> SourceB;


        public override async Task ProcessInput(object input , Metadata metadata)
        {
            if (input is Data<In1> && SourceA.Contains(metadata.SenderId))
            {
                var result = await ProcessData((Data<In1>)input, metadata);
                foreach (var output in result)
                {
                    await SendToNextStreamData(output.Key, output.Value, GetMetadata());
                }
            }
            else if (input is Data<In2> && SourceB.Contains(metadata.SenderId))
            {
                var result = await ProcessData((Data<In2>)input, metadata);
                foreach (var output in result)
                {
                    await SendToNextStreamData(output.Key, output.Value, GetMetadata());
                }
            }
            else throw new ArgumentException("Argument is not of type " + typeof(In1).FullName + " or " + typeof(In2).FullName);
        }

        public abstract Task<List<Data<Out>>> ProcessData(Data<In1> input, Metadata metadata);
        public abstract Task<List<Data<Out>>> ProcessData(Data<In2> input, Metadata metadata);

        public Task SetSources(List<Guid> sourceA, List<Guid> sourceB)
        {
            SourceA = sourceA;
            SourceB = sourceB;
            return Task.CompletedTask;
        }
    }
}
