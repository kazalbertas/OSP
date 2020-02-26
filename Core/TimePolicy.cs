using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP
{
    public enum TimePolicy 
    { 
        None,
        EventTime,
        ProcessingTime,
        IngestionTime
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
