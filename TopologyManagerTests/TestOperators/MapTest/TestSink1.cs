using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestOperators.MapTest
{
    public class TestSink1 : Sink<TypeB>
    {
        int index = 0;
        public override void Consume(TypeB input)
        {
            if (index == 0)
            {
                Assert.Equal("0", input.Field2);
                index++;
            }
            else 
            {
                Assert.Equal("1", input.Field2);
            }
        }
    }
}
