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
                    StaticTestHelper.FailTest("Wrong input received, expected: 0 |actual: " + input.Field2);
                }
                else
                {
                    StaticTestHelper.PassTest("Expected input received, expected: 0 | actual: " + input.Field2);
                }
                
                index++;
            }
            else 
            {
                StaticTestHelper.TempFailTest("Temp fail test for index 1");

                if (!input.Field2.Equals("1"))
                {

                    StaticTestHelper.FailTest("Wrong input received, expected: 1 |actual: " + input.Field2);
                }
                else
                {
                    StaticTestHelper.PassTest("Expected input received, expected: 1 | actual: " + input.Field2);
                }
            }
        }
    }
}
