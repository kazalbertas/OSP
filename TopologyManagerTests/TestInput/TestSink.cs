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
            if (!success) GrainFactory.GetGrain<ITestHelper>(0).ShouldBreak();
            
            if (input == "Test1") 
            {
                gotTest1 = true;
            }
            if (input == "Test2") 
            {
                gotTest2 = true;
            }

            if (gotTest2 && gotTest1) success = true;
            if (success) GrainFactory.GetGrain<ITestHelper>(0).Reset();
        }
    }
}
