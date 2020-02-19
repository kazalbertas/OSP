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
            var testHelper = GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);

            if (index == 0)
            {
                if (!input.Field2.Equals("0")) 
                {
                    StaticTestHelper.FailTest("Wrong input received, expected: 0 |actual: " + input.Field2);
                    //testHelper.FailTest("Wrong input received, expected: 0 |actual: " + input.Field2).RunSynchronously();
                }
                else
                {
                    StaticTestHelper.PassTest("Expected input received, expected: 0 | actual: " + input.Field2);
                    //testHelper.PassTest("Expected input received, expected: 0 | actual: " + input.Field2).RunSynchronously();
                }
                
                index++;
            }
            else 
            {
                StaticTestHelper.TempFailTest("Temp fail test for index 1");
                //testHelper.TempFailTest("Temp fail test for index 1").RunSynchronously();

                if (!input.Field2.Equals("1"))
                {

                    StaticTestHelper.FailTest("Wrong input received, expected: 1 |actual: " + input.Field2);
                    //testHelper.FailTest("Wrong input received, expected: 1 |actual: " + input.Field2).RunSynchronously();
                }
                else
                {
                    //testHelper.PassTest("Expected input received, expected: 1 | actual: " + input.Field2).RunSynchronously();
                    StaticTestHelper.PassTest("Expected input received, expected: 1 | actual: " + input.Field2);
                }
            }
        }
    }
}
