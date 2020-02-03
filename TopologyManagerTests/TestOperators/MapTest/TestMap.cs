using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTests.TestOperators.MapTest
{
    public class TestMap : Map<TypeA, TypeB>
    {
        public override TypeB ApplyMap(TypeA input)
        {
            return new TypeB() { Field1 = input.Field1, Field2 = input.Field2.ToString() };
        }

        public override object GetKey(TypeB input)
        {
            return input.Field1;
        }
    }
}
