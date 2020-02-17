using CoreOSP.Partitioner;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSPTopologyManager.Operators.Configuration
{
    public class ConfigurationBase
    {
        public int OutputStreamCount { get; set; } = 1;
        public PartitionPolicy PartitionPolicy { get; set; } = PartitionPolicy.RoundRobin;
    }

    public class ConfigurationParallel : ConfigurationBase
    {
        public int Parallelism { get; set; } = 1;
    }
}
