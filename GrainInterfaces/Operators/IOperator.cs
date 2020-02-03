using CoreOSP.Models;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Operators
{
    public interface IOperator : IGrainWithGuidKey
    {
        Task Process((object, Metadata) packedInput, StreamSequenceToken sequenceToken);
        Task Init(Guid jobMgrId, Type jobMgrType, Type delegator);
        Task GetSubscribedStreams();
    }
}
