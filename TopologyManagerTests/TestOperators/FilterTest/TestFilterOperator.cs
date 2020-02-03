using CoreOSP.Delegators;
using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSPTests.TestOperators.FilterTest
{
    [Collection(ClusterCollection.Name)]
    public class TestFilterOperator
    {
        private readonly TestCluster _cluster;

        public TestFilterOperator(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public void TestSourceFilterSinkTopology()
        {
            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1));

            var sourceGuid = ds.OperatorId;
            Type sourceType = ds.OperatorType;

            var df = ds.Filter(typeof(TestFilter));

            var filterGuid = df.OperatorId;
            var filterType = df.OperatorType;

            df.Sink(typeof(TestSink1));

            Assert.Equal(sourceGuid, ds.OperatorId);
            Assert.Equal(sourceType, ds.OperatorType);

            Assert.Equal(filterGuid, ds.Next.OperatorId);
            Assert.Equal(filterType, ds.Next.OperatorType);

            Assert.Equal(typeof(TestSink1), ds.Next.Next.OperatorType);
            Assert.Null(ds.Next.Next.Next);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestSourceFilterSinkRunAsync()
        {
            var conf = new TopologyConfiguration();
            conf.Delegator = typeof(RoundRobinDelegator);
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1));
            ds.Filter(typeof(TestFilter)).Sink(typeof(TestSink1));

            JobManager jmgr = new JobManager();
            await jmgr.StartTestJobAsync(mgr, _cluster.Client);
        }
    }
}
