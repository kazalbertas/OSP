using CoreOSP;
using System.Threading.Tasks;

namespace GrainInterfaces.Operators
{
    public interface ISource : IOperator
    {
        public Task Start();

        public Task InitSource(TimePolicy policy);
    }
}
