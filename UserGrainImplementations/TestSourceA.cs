using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserGrainImplementations
{
    public class TestSourceA : SourceSMSProvider<string>
    {
        public override object GetKey(string input)
        {
            return "abc";
        }

        public override Guid GetStreamID()
        {
            return new Guid();
        }

        public override string GetStreamName()
        {
            return "Tag";
        }

        public override string ProcessMessage(string message)
        {
            return message;
        }
    }
}
