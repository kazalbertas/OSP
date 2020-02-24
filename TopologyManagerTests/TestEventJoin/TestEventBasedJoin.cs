﻿using CoreOSP.Models;
using GrainImplementations.Operators.Join;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSPTests.TestEventJoin
{
    public class TestEventBasedJoin : EventBasedJoin<Test,Test2>
    {
        public override DateTime ExtractDateTime(Test input)
        {
            return input.EventTime;
        }

        public override DateTime ExtractDateTime(Test2 input)
        {
            return input.EventTime;
        }

        public override Task ProcessTerminationEvent(Data<TerminationEvent> tevent)
        {
            StaticTestHelper.LogMessage(this.sourceAInput.Count.ToString());
            StaticTestHelper.LogMessage(this.sourceBInput.Count.ToString());
            StaticTestHelper.LogMessage(tevent.Value.Key.ToString());
            base.ProcessTerminationEvent(tevent);
            StaticTestHelper.LogMessage(this.sourceAInput.Count.ToString());
            StaticTestHelper.LogMessage(this.sourceBInput.Count.ToString());
            return Task.CompletedTask;
        }
    }
}
