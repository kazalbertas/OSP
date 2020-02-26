using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserGrainImplementations
{
    public class TestSourceA : SourceSMSProvider<string>
    {
        public override DateTime ExtractEventTime(string data)
        {
            throw new NotImplementedException();
        }

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

        public override TimeSpan MaxOutOfOrder()
        {
            throw new NotImplementedException();
        }

        public override string ProcessMessage(string message)
        {
            return message;
        }

        public override TimeSpan WatermarkIssuePeriod()
        {
            throw new NotImplementedException();
        }
    }
}
