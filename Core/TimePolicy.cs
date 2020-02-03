using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP
{
    public enum TimePolicy 
    { 
        EventTime,
        ProcessingTime
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class EventTime : Attribute 
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ProcessingTime : Attribute
    {

    }


}
