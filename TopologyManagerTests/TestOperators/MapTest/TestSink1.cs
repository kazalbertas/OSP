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
                if (!input.Field2.Equals("0")) 
                {
                    GrainFactory.GetGrain<ITestHelper>(0).ShouldBreak();
                }
                
                index++;
            }
            else 
            {
                if (!input.Field2.Equals("1"))
                {
                    GrainFactory.GetGrain<ITestHelper>(0).ShouldBreak();
                }
            }
        }
    }
}
