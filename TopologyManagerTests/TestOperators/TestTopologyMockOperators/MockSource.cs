﻿using GrainImplementations.Operators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSPTests.TestOperators.TestTopologyMockOperators
{
    public class MockSource : Source<int>
    {
        public override DateTime ExtractEventTime(int data)
        {
            throw new NotImplementedException();
        }

        public override object GetKey(int input)
        {
            throw new NotImplementedException();
        }

        public override TimeSpan MaxOutOfOrder()
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

        public override TimeSpan WatermarkIssuePeriod()
        {
            throw new NotImplementedException();
        }
    }
}
