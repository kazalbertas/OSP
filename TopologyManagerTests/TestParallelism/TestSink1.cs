﻿using GrainImplementations.Operators;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestParallelism
{
    
    public class TestSink1 : Sink<string>
    {
        private string val = "";

        public override void Consume(string input)
        {
            if (val == "") val = input;

            if (val != input)
            {
                GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace).FailTest("Unexpected value received");
            }
            else
            {
                GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace).PassTest("Value received");
            }
        }
    }
}
