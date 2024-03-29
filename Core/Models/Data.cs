﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreOSP.Models
{
    public class Data<V>
    {
        public object Key { get; set; }
        public V Value { get; set; }
 
        public DateTime ProcessingTime { get; set; }

        public Data(object key, V val)
        {
            Key = key;
            Value = val;
            ProcessingTime = DateTime.Now;
        }

        public DateTime GetTime(TimePolicy policy) 
        {
            DateTime time;
            switch (policy) 
            {
                case TimePolicy.EventTime:
                    var prop = this.Value.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(EventTime))).FirstOrDefault();
                    var value = prop.GetValue(Value, null);
                    if (value == null) throw new FieldAccessException("Data type does not contain event time");
                    time = (DateTime)value;
                    break;
                case TimePolicy.ProcessingTime:
                    time = ProcessingTime;
                    break;
                default: 
                    throw new ArgumentException("Such Time policy not suported");
            }
            return time;
        }
    }
}
