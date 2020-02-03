using CoreOSP.Exceptions;
using GrainInterfaces.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace TopologyManagerOSP.Operators
{
    public class DataStream
    {
        public DataStream Prev { get; private set; }
        public DataStream Next { get; private set; }

        public List<Guid> OperatorId { get; private set; } = new List<Guid>();
        public Type OperatorType { get; private set; }
        public object Configuration { get; private set; }
        private int _parallelism = 1;

        internal DataStream(Type t, int parallelism)
        {
            for (int i = 0; i < parallelism; i++)
            {
                OperatorId.Add(Guid.NewGuid());
            }
            _parallelism = parallelism;
            OperatorType = t;
        }
        internal DataStream(DataStream previous, Type t, int parallelism)
        {
            for (int i = 0; i < parallelism; i++)
            {
                OperatorId.Add(Guid.NewGuid());
            }
            _parallelism = parallelism;
            Prev = previous;
            OperatorType = t;
        }

        public DataStream Map(Type t)
        {
            if (!DataStreamValidator.ValidateType<IMap>(t)) new OperatorMismatchException("Operator is not of type IMap");

            var next = new DataStream(this, t, _parallelism);
            Next = next;
            return next;
        }

        public DataStream FlatMap(Type t)
        {
            if (!DataStreamValidator.ValidateType<IFlatMap>(t)) new OperatorMismatchException("Operator is not of type IFlatMap");

            var next = new DataStream(this, t, _parallelism);
            Next = next;
            return next;
        }

        //public DataStream WindowJoin(Type t)
        //{

        //}

        //public DataStream WindowAggregate(Type t)
        //{

        //}

        public void Sink(Type t)
        {
            if (!DataStreamValidator.ValidateType<ISink>(t)) new OperatorMismatchException("Operator is not of type ISink");
            var next = new DataStream(this, t, _parallelism);
            Next = next;
        }

        public DataStream Filter(Type t)
        {
            if (!DataStreamValidator.ValidateType<IFilter>(t)) new OperatorMismatchException("Operator is not of type IFilter");

            var next = new DataStream(this, t, _parallelism);
            Next = next;
            return next;
        }
    }
}
