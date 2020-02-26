using CoreOSP;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTopologyManager
{
    public class TopologyConfiguration
    {
        public TimePolicy TimeCharacteristic { get; set; } = TimePolicy.ProcessingTime;
        public bool CheckpointEnabled { get; set; } = false;
        public TimeSpan CheckpointInterval { get; set; } = TimeSpan.MinValue;
        public ProcessingType ProcessingType { get; set; } = ProcessingType.SynchronizeEach;
    }
}
