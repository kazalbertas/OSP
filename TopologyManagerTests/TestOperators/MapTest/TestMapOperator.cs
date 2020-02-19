using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Threading;
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

        //[Fact]
        //public void TestMapTopology()
        //{
        //    var conf = new TopologyConfiguration();
        //    var mgr = new TopologyManager(conf);
        //    var ds = mgr.AddSource(typeof(TestSource1));

        //    var sourceGuid = ds.OperatorGUIDs;
        //    Type sourceType = ds.OperatorType;

        //    var df = ds.Map(typeof(TestMap));

        //    var mapGuid = df.OperatorGUIDs;
        //    var mapType = df.OperatorType;

        //    df.Sink(typeof(TestSink1));

        //    Assert.Equal(sourceGuid, df.Prev.Prev.OperatorGUIDs);
        //    Assert.Equal(sourceType, df.Prev.Prev.OperatorType);

        //    Assert.Equal(mapGuid, df.Prev.OperatorGUIDs);
        //    Assert.Equal(mapType, df.Prev.OperatorType);

        //    Assert.Equal(typeof(TestSink1), df.OperatorType);
        //}

        [Fact]
        public async System.Threading.Tasks.Task TestMap()
        {
            StaticTestHelper.Reset();
            StaticTestHelper.TempFailTest("Initial fail of the test");

            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1));
            ds.Map(typeof(TestMap)).Sink(typeof(TestSink1));

            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(7000);
            var result = StaticTestHelper.GetStatus();
            Assert.False(result.Item1, result.Item2);
        }
    }
}
