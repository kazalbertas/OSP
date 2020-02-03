using CoreOSP.Partitioner;
using System;
using System.Collections.Generic;
using System.Text;
using TopologyManagerOSP.Operators;

namespace OSPTopologyManager
{
    public class TopologyManager
    {

        public List<DataStream> Operators { get; private set; } = new List<DataStream>();

        public TopologyConfiguration Conf { get; private set; }

        public TopologyManager(TopologyConfiguration conf) 
        {
            Conf = conf;
        }

        public DataStream AddSource(Type source, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin) 
        {
            var ds = new DataStream(this, source, outputStreamCount, partitionPolicy);
            return ds;
        }

    }
}
