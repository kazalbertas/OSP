using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP
{
    public class OperatorInitConfig
    {
        public Guid JobManagerGuid { get; set; }
        public Type JobManagerType { get; set; }
        public TimePolicy TimeCharacteristic { get; set; }
        public ProcessingType ProcessingType { get; set; }
        public Type Partitioner { get; set; }
    }
}
