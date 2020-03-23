using CoreOSP.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Operators
{
    public interface IStorage : IOperator
    {
        Task<object> Select(string functionName, object parameters);

        Task<string> GetName();

        Task<List<string>> GetFunctions();
    }
}
