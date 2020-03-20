using CoreOSP.Models;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Synchronous
{
    public abstract class SingleInputStreamOperator<In,Out> : Operator
    {
        public override async Task ProcessInput(object input, Metadata metadata)
        {
            if (input is Data<In>)
            {
                var result = await ProcessData((Data<In>)input, metadata);
                foreach (var output in result)
                {
                    await SendToNextStreamData(output.Key, output.Value, GetMetadata());
                }
            }
            else throw new ArgumentException("Argument is not of type " + typeof(In).FullName);
        }

        public abstract Task<List<Data<Out>>> ProcessData(Data<In> input, Metadata metadata);
    }
}
