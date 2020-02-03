using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP.Delegators
{
    public interface IDelegator
    {
        void SetNextOperators(List<(Guid, Type)> op);
        (Guid, Type) DelegateToProcess(object key);
    }
}
