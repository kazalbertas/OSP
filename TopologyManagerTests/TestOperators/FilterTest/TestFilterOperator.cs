using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

        //[Fact]
        //public void TestSourceFilterSinkTopology()
        //{
        //    var conf = new TopologyConfiguration();
        //    var mgr = new TopologyManager(conf);
        //    var ds = mgr.AddSource(typeof(TestSource1));

        //    var sourceGuid = ds.OperatorGUIDs;
        //    Type sourceType = ds.OperatorType;

        //    var df = ds.Filter(typeof(TestFilter));

        //    var filterGuid = df.OperatorGUIDs;
        //    var filterType = df.OperatorType;

        //    df.Sink(typeof(TestSink1));

        //    Assert.Equal(filterGuid, df.Prev.OperatorGUIDs);
        //    Assert.Equal(filterType, df.Prev.OperatorType);

        //    Assert.Equal(sourceGuid, df.Prev.Prev.OperatorGUIDs);
        //    Assert.Equal(sourceType, df.Prev.Prev.OperatorType);

        //    Assert.Equal(sourceGuid, ds.OperatorGUIDs);
        //    Assert.Equal(sourceType, ds.OperatorType);

        //    Assert.Equal(typeof(TestSink1), df.OperatorType);

        //}

        [Fact]
        public async System.Threading.Tasks.Task TestSourceFilterSinkRunAsync()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            await breaker.Reset();
            await breaker.TempFailTest("Initial fail of test");
            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1));
            ds.Filter(typeof(TestFilter)).Sink(typeof(TestSink1));

            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(1000);
            var result = await breaker.GetStatus();
            Assert.False(result.Item1, result.Item2);
        }
    }
}
