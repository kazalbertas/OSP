using CoreOSP.Models;
using GrainInterfaces.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace GrainImplementations.Operators
{
    public abstract class Storage<T> : Operator<T>, IStorage
    {
        private Dictionary<string, Func<Dictionary<object, List<Data<T>>>, object, object>> functions = new Dictionary<string, Func<Dictionary<object, List<Data<T>>>, object, object>>();
        private Dictionary<object, List<Data<T>>> storedValues { get; set; } = new Dictionary<object, List<Data<T>>>();

        public override Task OnActivateAsync()
        {
            base.OnActivateAsync();
            functions = SetFunctions();
            return Task.CompletedTask;
        }
        public override async Task ProcessData(Data<T> input, Metadata metadata)
        {
            if (!storedValues.ContainsKey(input.Key))
            {
                storedValues.Add(input.Key, new List<Data<T>>() { input });
            }
            else
            {
                storedValues[input.Key].Add(input);
            }
            await SendToNextStreamData(input.Key, input, GetMetadata());
        }

        public abstract Dictionary<string, Func<Dictionary<object, List<Data<T>>>, object, object>> SetFunctions();

        public Task<object> Select(string functionName, object parameters) 
        {
            return Task.FromResult(functions[functionName].Invoke(storedValues,parameters));
        }

        public abstract Task<string> GetName();
        public Task<List<string>> GetFunctions()
        {
            return Task.FromResult(functions.Keys.ToList());
        }
    }
}
