using CoreOSP.Exceptions;
using CoreOSP.Partitioner;
using GrainInterfaces.Operators;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace TopologyManagerOSP.Operators
{
    public class DataStream
    {
        public DataStream Prev { get; private set; }

        public List<Guid> OperatorGUIDs { get; set; } = new List<Guid>();
        public Type OperatorType { get; set; }
        public object Configuration { get; private set; }

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
        internal DataStream(TopologyManager mgr, Type t, int outputStreamCount, PartitionPolicy partitionPolicy)
        {
            OperatorGUIDs.Add(Guid.NewGuid());
            OperatorType = t;
            StreamGUID = Guid.NewGuid();
            SetPartitioner(partitionPolicy);
            OutputStreamCount = outputStreamCount;
            _mgr = mgr;
            mgr.Operators.Add(this);
        }

        internal DataStream(TopologyManager mgr, DataStream previous, Type t, int parallelism, int outputStreamCount, PartitionPolicy partitionPolicy)
        {
            for (int i = 0; i < parallelism; i++)
            {
                OperatorGUIDs.Add(Guid.NewGuid());
            }
            Prev = previous;
            OperatorType = t;
            StreamGUID = Guid.NewGuid();
            Parallelism = parallelism;
            SetPartitioner(partitionPolicy);
            OutputStreamCount = outputStreamCount;
            _mgr = mgr;
            mgr.Operators.Add(this);
        }

        public DataStream Map(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<IMap>(t)) new OperatorMismatchException("Operator is not of type IMap");

            return new DataStream(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        public DataStream FlatMap(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<IFlatMap>(t)) new OperatorMismatchException("Operator is not of type IFlatMap");

            return new DataStream(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        //public DataStream WindowJoin(Type t)
        //{

        //}

        //public DataStream WindowAggregate(Type t)
        //{

        //}

        public void Sink(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<ISink>(t)) new OperatorMismatchException("Operator is not of type ISink");
            new DataStream(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        public DataStream Filter(Type t, int parallelism = 1, int outputStreamCount = 1, PartitionPolicy partitionPolicy = PartitionPolicy.RoundRobin)
        {
            if (!DataStreamValidator.ValidateType<IFilter>(t)) new OperatorMismatchException("Operator is not of type IFilter");

            return new DataStream(_mgr, this, t, parallelism, outputStreamCount, partitionPolicy);
        }

        private void SetPartitioner(PartitionPolicy partitionPolicy)
        {
            switch (partitionPolicy)
            {
                case PartitionPolicy.RoundRobin:
                    Partitioner = typeof(RoundRobinPartitioner);
                    break;
                case PartitionPolicy.Random:
                    Partitioner = typeof(RandomPartitioner);
                    break;
                case PartitionPolicy.Key:
                    Partitioner = typeof(KeyPartitioner);
                    break;
                default: throw new ArgumentOutOfRangeException("Not supported partitioning function");

            }

        }
    }
}
