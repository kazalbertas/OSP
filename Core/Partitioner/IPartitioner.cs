using CoreOSP.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP.Partitioner
{
    public interface IPartitioner
    {
        void SetOutputStreams(Guid streamGuid, List<int> streamIDs);

        (Guid,int) GetNextStream(object key);
    }

    public class RoundRobinPartitioner : IPartitioner
    {
        private int _streamsCount = 0;
        private int _nextStreamidx = 0;

        private Guid _streamGuid;
        private List<int> _streamIDs;

        public (Guid, int) GetNextStream(object key)
        {
            if (_streamsCount == _nextStreamidx)
            {
                _nextStreamidx = 0;
            }
            var op = _streamIDs[_nextStreamidx];
            _nextStreamidx++;
            return (_streamGuid, op);
        }

        public void SetOutputStreams(Guid streamGuid, List<int> streamIDs)
        {
            _streamGuid = streamGuid;
            _streamIDs = streamIDs;
            _streamsCount = _streamIDs.Count;
        }
    }

    public class KeyPartitioner : IPartitioner
    {
        public (Guid, int) GetNextStream(object key)
        {
            throw new NotImplementedException();
        }

        public void SetOutputStreams(Guid streamGuid, List<int> streamIDs)
        {
            throw new NotImplementedException();
        }
    }

    public class RandomPartitioner : IPartitioner
    {
        public (Guid, int) GetNextStream(object key)
        {
            throw new NotImplementedException();
        }

        public void SetOutputStreams(Guid streamGuid, List<int> streamIDs)
        {
            throw new NotImplementedException();
        }
    }
}
