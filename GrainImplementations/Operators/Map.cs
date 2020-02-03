using CoreOSP.Models;
using GrainInterfaces.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Operators
{
    public abstract class Map<T,K> : Operator<T>, IMap
    {

        public override void ProcessData(Data<T> input, Metadata metadata)
        {
            var result = ApplyMap(input.Value);
            (var _nextOperatorId, var _nextOperatorClass) = _delegator.DelegateToProcess(input.Key);
            var dt = new Data<K>(GetKey(result), result);
            GrainFactory.GetGrain<IOperator>(_nextOperatorId, _nextOperatorClass.FullName).Process(dt, GetMetadata());
        }

        public abstract K ApplyMap(T input);

        public abstract object GetKey(K input);

    }
}
