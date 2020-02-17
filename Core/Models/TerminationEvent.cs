using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP.Models
{
    public enum TerminationEventType 
    {
        SourceInjected,
        DirectlyInjected
    }

    public class TerminationEvent
    {
        public object Key { get; set; }
    }
}
