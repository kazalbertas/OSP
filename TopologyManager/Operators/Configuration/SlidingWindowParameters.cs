using System;
using System.Collections.Generic;
using System.Text;

namespace TopologyManagerOSP.Operators.Configuration
{
    public class SlidingWindowParameters
    {
        public TimeSpan WindowSize { get; set; }
        public TimeSpan SlideSize { get; set; }
    }
}
