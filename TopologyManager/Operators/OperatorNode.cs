using CoreOSP.Exceptions;
using CoreOSP.Partitioner;
using GrainInterfaces.Operators;
using OSPTopologyManager;
using OSPTopologyManager.Operators.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TopologyManagerOSP.Operators
{
    //rename to OperatorNode
    public class OperatorNode
    {
        // make a list
        public List<OperatorNode> Prev { get; private set; } = new List<OperatorNode>();
        public List<OperatorNode> SourceBPrev { get; private set; } = new List<OperatorNode>();

        public List<Guid> OperatorGUIDs { get; set; } = new List<Guid>();
        public Type OperatorType { get; set; }
        public ConfigurationBase Configuration { get; private set; }

        public int Parallelism { get; set; }
        public Guid StreamGUID { get; set; }
        public int OutputStreamCount { get; set; }
        public Type Partitioner { get; set; }

        private TopologyManager _mgr;


        /// <summary>
        /// Only used when adding source operator, because it cannot be paralelized. 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="mgr">Topology manager reference for saving new operators</param>
        internal OperatorNode(TopologyManager mgr, Type t, int outputStreamCount, PartitionPolicy partitionPolicy)
        {
            OperatorGUIDs.Add(Guid.NewGuid());
            OperatorType = t;
            StreamGUID = Guid.NewGuid();
            SetPartitioner(partitionPolicy);
            OutputStreamCount = outputStreamCount;
            _mgr = mgr;
            mgr.Operators.Add(this);
        }

        internal OperatorNode(TopologyManager mgr, OperatorNode previous, Type t, int parallelism, int outputStreamCount, PartitionPolicy partitionPolicy)
        {
            for (int i = 0; i < parallelism; i++)
            {
                OperatorGUIDs.Add(Guid.NewGuid());
            }
            Prev.Add(previous);
            OperatorType = t;
            StreamGUID = Guid.NewGuid();
            Parallelism = parallelism;
            SetPartitioner(partitionPolicy);
            OutputStreamCount = outputStreamCount;
            _mgr = mgr;
            mgr.Operators.Add(this);
        }

        internal OperatorNode(TopologyManager mgr, OperatorNode previous, Type t, int parallelism, int outputStreamCount, PartitionPolicy partitionPolicy, OperatorNode sourceB)
        {
            for (int i = 0; i < parallelism; i++)
            {
                OperatorGUIDs.Add(Guid.NewGuid());
            }
            Prev.Add(previous);
            OperatorType = t;
            StreamGUID = Guid.NewGuid();
            Parallelism = parallelism;
            SetPartitioner(partitionPolicy);
            OutputStreamCount = outputStreamCount;
            _mgr = mgr;
            SourceBPrev.Add(sourceB);
            mgr.Operators.Add(this);
        }

        public OperatorNode Map(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<IMap>(t)) new OperatorMismatchException("Operator is not of type IMap");

            return new OperatorNode(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        public OperatorNode FlatMap(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<IFlatMap>(t)) new OperatorMismatchException("Operator is not of type IFlatMap");

            return new OperatorNode(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        public OperatorNode WindowJoin(Type t, OperatorNode sourceB, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<IWindowJoin>(t)) new OperatorMismatchException("Operator is not of type IWindowJoin");
            return new OperatorNode(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy, sourceB);
        }

        public OperatorNode WindowAggregate(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<IWindowAggregation>(t)) new OperatorMismatchException("Operator is not of type IWindowAggregation");
            return new OperatorNode(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        public OperatorNode Sink(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<ISink>(t)) new OperatorMismatchException("Operator is not of type ISink");
            return new OperatorNode(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        public OperatorNode Filter(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<IFilter>(t)) new OperatorMismatchException("Operator is not of type IFilter");

            return new OperatorNode(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        public OperatorNode Storage(Type t, int outputStreamCount, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            // needs a check
            //if (!DataStreamValidator.ValidateType<IStorage<T>>(t)) new OperatorMismatchException("Operator is not of type IFilter");

            return new OperatorNode(_mgr, this, t, 1, outputStreamCount, partitionPolicy);
        }

        public void AddInput(OperatorNode node) 
        {
            Prev.Add(node);
        }

        private void SetPartitioner(PartitionPolicy partitionPolicy)
        {
            Partitioner = partitionPolicy switch
            {
                PartitionPolicy.RoundRobin => typeof(RoundRobinPartitioner),
                PartitionPolicy.Random => typeof(RandomPartitioner),
                PartitionPolicy.Key => typeof(KeyPartitioner),
                _ => throw new ArgumentOutOfRangeException("Not supported partitioning function"),
            };
        }
    }
}
