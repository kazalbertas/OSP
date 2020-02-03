using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UserGrainImplementations
{
    public class TestSource : Source<int>
    {
        public override object GetKey(int input)
        {
            throw new NotImplementedException();
        }

        public override int ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override Task Start()
        {
            throw new NotImplementedException();
        }
    }
}
