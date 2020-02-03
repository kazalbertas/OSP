using CoreOSP.Models;
using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSPTests.TestParallelism
{
    public class TestSource1 : Source<string>
    {
        public override object GetKey(string input)
        {
            return input;
        }

        public override string ProcessMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override async Task Start()
        {
            Data<string> dt = new Data<string>(GetKey("TestKey"), "Test2");
            SendToNextStreamAsync(dt.Key, dt, GetMetadata());

            Data<string> dt2 = new Data<string>(GetKey("TestKey"), "Test1");
            SendToNextStreamAsync(dt2.Key, dt2, GetMetadata());

            Data<string> dt3 = new Data<string>(GetKey("TestKey"), "Test2");
            SendToNextStreamAsync(dt3.Key, dt3, GetMetadata());

            Data<string> dt4 = new Data<string>(GetKey("TestKey"), "Test2");
            SendToNextStreamAsync(dt4.Key, dt4, GetMetadata());
        }
    }
}
