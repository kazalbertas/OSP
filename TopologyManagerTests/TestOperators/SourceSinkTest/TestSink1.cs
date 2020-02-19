using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestOperators.SourceSinkTest
{
    public class TestSink1 : Sink<string>
    {
        int index = 0;
        public override void Consume(string input)
        {
            var testHelper = GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);

            if (index == 0)
            {
                if (!"Test1".Equals(input))
                {
                    testHelper.FailTest("Wrong input received, expected Test1 | got: " + input).RunSynchronously();
                }
                else 
                {
                    testHelper.PassTest("Correct value received").RunSynchronously();
                }
                index++;
            }
            else 
            {
                testHelper.TempFailTest("Temp fail for index 2 ").RunSynchronously();
                if (!"Test2".Equals(input))
                {
                    testHelper.FailTest("Wrong input received, expected Test2 | got: " + input).RunSynchronously();
                }
                else
                {
                    testHelper.PassTest("Correct value received").RunSynchronously();
                }
            }
        }
    }
}
