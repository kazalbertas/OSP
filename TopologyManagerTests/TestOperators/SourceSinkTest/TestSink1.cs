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
            if (index == 0)
            {
                if (!"Test1".Equals(input))
                {
                    StaticTestHelper.FailTest("Wrong input received, expected Test1 | got: " + input);
                }
                else 
                {
                    StaticTestHelper.PassTest("Correct value received");
                }
                index++;
            }
            else 
            {
                StaticTestHelper.TempFailTest("Temp fail for index 2 ");
                if (!"Test2".Equals(input))
                {
                    StaticTestHelper.FailTest("Wrong input received, expected Test2 | got: " + input);
                }
                else
                {
                    StaticTestHelper.PassTest("Correct value received");
                }
            }
        }
    }
}
