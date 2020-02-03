using CoreOSP.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Operators
{
    public interface IOperator : IGrainWithGuidKey
    {
        Task Process(object input, Metadata metadata);
        Task Init(Guid jobMgrId, Type jobMgrType, Type delegator);
    }
}
