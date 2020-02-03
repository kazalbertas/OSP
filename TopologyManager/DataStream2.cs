//using CoreOSP.Partitioner;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace OSPTopologyManager
//{
//    public class DataStream
//    {
//        public List<Guid> OperatorGUID { get; set; } = new List<Guid>();
//        public Type OperatorType { get; set; }

//        public int Parallelism { get; set; }
//        public Type Partitioner { get; set; }


//        public DataStream PreviousOperator { get; set; }

//        internal DataStream(DataStream previousOperator) 
//        {
//            PreviousOperator = previousOperator;
//        }

//        public DataStream Map(Type t, PartitionPolicy partitionPolicy)
//        {
//            SetPartitioner(partitionPolicy);
//            OperatorGUID = Guid.NewGuid();
//            OperatorType = t;
//            return new DataStream(this);
//        }

//        private void SetPartitioner(PartitionPolicy partitionPolicy) 
//        {
//            switch (partitionPolicy) 
//            {
//                case PartitionPolicy.RoundRobin:
//                    Partitioner = typeof(RoundRobinPartitioner);
//                    break;
//                case PartitionPolicy.Random:
//                    Partitioner = typeof(RandomPartitioner);
//                    break;
//                case PartitionPolicy.Key:
//                    Partitioner = typeof(KeyPartitioner);
//                    break;
//                default: throw new ArgumentOutOfRangeException("Not supported partitioning function");
            
//            }
        
//        }
//    }

//    public class WindowDataStream : DataStream
//    {
//        public int WindowSize { get; set; }
//        public int WindowSlide { get; set; }

//        internal WindowDataStream(DataStream previousOperator, int windowSize, int windowSlide) : base(previousOperator)
//        {
//            WindowSize = windowSize;
//            WindowSlide = windowSlide;
//        }
//    }

//    public class JoinDataStream : WindowDataStream
//    {
//        public DataStream PreviousOperatorJoined { get; set; }

//        internal JoinDataStream(DataStream previousOperator, DataStream joinOperator , int windowSize, int windowSlide) : base(previousOperator, windowSize, windowSlide)
//        {
//            PreviousOperatorJoined = joinOperator;
//        }
//    }
//}
