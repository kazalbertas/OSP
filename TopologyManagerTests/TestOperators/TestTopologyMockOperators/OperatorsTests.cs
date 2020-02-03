using CoreOSP.Models;
using GrainImplementations.Operators;

using OSPTopologyManager;
using System;
using TopologyManagerOSP;
using Xunit;

namespace OSPTests.TestOperators.TestTopologyMockOperators
{
    public class OperatorsTests
    {
        //[Fact]
        //public void TestTopologyManager()
        //{
        //    var conf = new TopologyConfiguration();
        //    var mgr = new TopologyManager(conf);
        //    var ds = mgr.AddSource(typeof(MockSource));

        //    var sourceGuid = ds.OperatorId;
        //    Type sourceType = ds.OperatorType;

        //    var df = ds.Filter(typeof(MockFilter));

        //    var filterGuid = df.OperatorId;
        //    var filterType = df.OperatorType;

        //    df.Sink(typeof(MockSink));

        //    Assert.Equal(sourceGuid, ds.OperatorId);
        //    Assert.Equal(sourceType, ds.OperatorType);

        //    Assert.Equal(filterGuid, ds.Next.OperatorId);
        //    Assert.Equal(filterType, ds.Next.OperatorType);

        //    Assert.Equal(typeof(MockSink), ds.Next.Next.OperatorType);
        //    Assert.Null(ds.Next.Next.Next);
        //}

        [Fact]
        public void TestHashing() 
        {
            var a = new Data<int>("abc",2);
            var b = new Data<int>("abc", 3);

            Assert.Equal(a.Key.GetHashCode(), b.Key.GetHashCode());
        }
        


    }
}
