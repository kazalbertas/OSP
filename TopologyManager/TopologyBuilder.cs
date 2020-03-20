using GrainInterfaces.Operators;
using OSPTopologyManager.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopologyManagerOSP.Operators;

namespace OSPTopologyManager
{
    public class Topology 
    {
        public List<Topology> Prev { get; set; }
        public OperatorNode Curr { get; set; }

        public List<Topology> Next { get; set; }
    }

    public class TopologyBuilder
    {

        //public TopologyManager OptimizeTopology(TopologyManager tpm) 
        //{
        //    List<Topology> tp = new List<Topology>();
        //    foreach(var op in tpm.Operators)
        //    {
        //        if (op.OperatorType.GetInterfaces().Contains(typeof(ISource))) 
        //        {
        //            var t = new Topology();
        //            t.Prev = new List<Topology>();
        //            t.Next = new List<Topology>();
        //            t.Curr = op;
        //            tp.Add(t);
        //        }
        //        else 
        //        {
        //            var t = new Topology();
        //            t.Prev = new List<Topology>();
        //            t.Next = new List<Topology>();
        //            t.Curr = op;

        //            tp.fin


        //        }

        //    }
        //}
    }
}
