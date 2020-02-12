using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Operators
{
    public interface IWindowJoin : IOperator
    {
        public Task SetSources(List<Guid> sourceA, List<Guid> sourceB);
    }
}
