using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserGrainImplementations
{
    public class GenericPrintSink : Sink<string>
    {
        public override void Consume(string input)
        {
            Console.WriteLine(input);
        }
    }
}
