using CoreOSP;
using CoreOSP.Models;
using Orleans;
using Orleans.Streams;
using System.Threading.Tasks;

namespace GrainInterfaces.Operators
{
    public interface IOperator : IGrainWithGuidKey
    {
        Task Process((object, Metadata) packedInput, StreamSequenceToken sequenceToken);
        Task Init(OperatorInitConfig operatorInitConfig);
        Task GetSubscribedStreams();
    }
}
