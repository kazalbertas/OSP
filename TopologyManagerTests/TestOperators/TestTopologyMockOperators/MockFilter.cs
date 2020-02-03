using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTests.TestOperators.TestTopologyMockOperators
{
    public class MockFilter : Filter<int>
    {
        public override bool Apply(int input)
        {
            throw new NotImplementedException();
        }
    }
}
