using CoreOSP.Delegators;
using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using Xunit;

namespace OSPTests.TestOperators.MapTest
{
    [Collection(ClusterCollection.Name)]
    public class TestMapOperator
    {
        private readonly TestCluster _cluster;

        public TestMapOperator(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public void TestMapTopology()
        {
            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1));

            var sourceGuid = ds.OperatorId;
            Type sourceType = ds.OperatorType;

            var df = ds.Map(typeof(TestMap));

            var mapGuid = df.OperatorId;
            var mapType = df.OperatorType;

            df.Sink(typeof(TestSink1));

            Assert.Equal(sourceGuid, ds.OperatorId);
            Assert.Equal(sourceType, ds.OperatorType);

            Assert.Equal(mapGuid, ds.Next.OperatorId);
            Assert.Equal(mapType, ds.Next.OperatorType);

            Assert.Equal(typeof(TestSink1), ds.Next.Next.OperatorType);
            Assert.Null(ds.Next.Next.Next);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestMap()
        {
            var conf = new TopologyConfiguration();
            conf.Delegator = typeof(RoundRobinDelegator);
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1));
            ds.Map(typeof(TestMap)).Sink(typeof(TestSink1));

            JobManager jmgr = new JobManager();
            await jmgr.StartTestJobAsync(mgr, _cluster.Client);
        }
    }
}
