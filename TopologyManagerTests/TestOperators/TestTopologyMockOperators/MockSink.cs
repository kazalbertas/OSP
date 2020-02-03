using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTests.TestOperators.TestTopologyMockOperators
{
    public class MockSink : Sink<int>
    {
        public override void Consume(int input)
        {
            throw new NotImplementedException();
        }
    }
}
