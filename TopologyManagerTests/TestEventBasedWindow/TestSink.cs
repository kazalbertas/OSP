using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestEventBasedWindow
{
    
    public class TestSink : Sink<int>
    {
        int count = 0;
        public override void Consume(int input)
        {

            if (input == 4 || input == 5 || input == 100 ||input == 8 || input == 10 )
            {
                StaticTestHelper.PassTest("Correct input received: " + input);
                count++;
            }
            else 
            {
                StaticTestHelper.FailTest("Completely incorrect value received: " + input );
            }

            if (count != 5) 
            {
                StaticTestHelper.TempFailTest("Count is not 5, actual: "+ count);
            }
        }
    }
}
