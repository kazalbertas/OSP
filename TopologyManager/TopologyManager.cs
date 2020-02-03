using System;
using System.Collections.Generic;
using System.Text;
using TopologyManagerOSP.Operators;

namespace OSPTopologyManager
{
    public class TopologyManager
    {

        public List<DataStream> Streams { get; private set; } = new List<DataStream>();

        public TopologyConfiguration Conf { get; private set; }

        public TopologyManager(TopologyConfiguration conf) 
        {
            Conf = conf;
        }

        public DataStream AddSource(Type source) 
        {
            var ds = new DataStream(source, Conf.Parallelism);
            Streams.Add(ds);
            return ds;
        }

    }
}
