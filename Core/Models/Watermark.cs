using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP.Models
{
    public class Watermark
    {
        public DateTime TimeStamp { get; set; }

        public Watermark(DateTime timeStamp) 
        {
            TimeStamp = timeStamp;
        }
    }
}
