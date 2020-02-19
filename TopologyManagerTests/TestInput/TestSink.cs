using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestInput
{
    
    public class TestSink : Sink<string>
    {

        private bool gotTest1 = false;
        private bool gotTest2 = false;

        private bool success = false;

        public override void Consume(string input)
        {
            if (!success) StaticTestHelper.TempFailTest("Not yet received from both inputs");

            if (input == "Test1")
            {
                gotTest1 = true;
            }
            else if (input == "Test2")
            {
                gotTest2 = true;
            }
            else
            {
                StaticTestHelper.FailTest("Unexpected input: " + input);
            }

            if (gotTest2 && gotTest1) success = true;
            if (success) StaticTestHelper.PassTest("Received from both inputs already");
        }
    }
}
