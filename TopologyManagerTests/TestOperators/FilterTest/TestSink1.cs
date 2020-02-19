﻿using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestOperators.FilterTest
{
    public class TestSink1 : Sink<string>
    {
        public override void Consume(string input)
        {
            if (!input.Equals("Test1")) 
            {
                GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace).FailTest("Wrong input received: " + input);
            } else 
            {
                GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace).PassTest("Correct input received");
            }
        }
    }
}
