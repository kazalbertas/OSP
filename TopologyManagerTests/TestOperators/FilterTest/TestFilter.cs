using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTests.TestOperators.FilterTest
{
    public class TestFilter : Filter<string>
    {
        public override bool Apply(string input)
        {
            return input == "Test1";
        }
    }
}
