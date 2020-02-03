using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP.Delegators
{
    [Obsolete]
    public class RoundRobinDelegator : IDelegator
    {
        private List<(Guid, Type)> nextOp = new List<(Guid, Type)>();

        private int _operatorsCount = 0;
        private int _nextOperatorIdx = 0;


        public (Guid, Type) DelegateToProcess(object key)
        {
            if (_operatorsCount == _nextOperatorIdx)
            {
                _nextOperatorIdx = 0;
            }
            var op = nextOp[_nextOperatorIdx];
            _nextOperatorIdx++;
            return op;
        }

        public List<(Guid, Type)> GetNextOperators()
        {
            return nextOp;
        }

        public void SetNextOperators(List<(Guid, Type)> op)
        {
            nextOp = op;
            _operatorsCount = op.Count;
            _nextOperatorIdx = 0;

        }
    }
}
