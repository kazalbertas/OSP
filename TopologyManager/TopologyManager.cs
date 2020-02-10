using CoreOSP.Partitioner;
using System;
using System.Collections.Generic;
using System.Text;
using TopologyManagerOSP.Operators;

namespace OSPTopologyManager
{
    public class TopologyManager
    {

        public List<OperatorNode> Operators { get; private set; } = new List<OperatorNode>();

        public TopologyConfiguration Conf { get; private set; }

        public TopologyManager(TopologyConfiguration conf) 
        {
            Conf = conf;
        }

        public OperatorNode AddSource(Type source, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin) 
        {
            var ds = new OperatorNode(this, source, outputStreamCount, partitionPolicy);
            return ds;
        }

    }
}
