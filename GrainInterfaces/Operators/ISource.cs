using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Operators
{
    public interface ISource : IOperator
    {
        public Task Start();

        public Task InitSource();
    }
}
