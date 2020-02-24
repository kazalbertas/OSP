using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OSPTests.TestEventJoin
{
    [Collection(ClusterCollection.Name)]
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
    public class TestEventBasedWindowJoin
    {
        private readonly TestCluster _cluster;

        public TestEventBasedWindowJoin(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async Task TestParallelAsync()
        {
            StaticTestHelper.Reset();
            StaticTestHelper.TempFailTest("Initial test failure");
            var conf = new TopologyConfiguration();
            conf.TimeCharacteristic = CoreOSP.TimePolicy.None;
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1), 1);

            var ds2 = mgr.AddSource(typeof(TestSource2));

            var wds = ds.WindowJoin(typeof(TestEventBasedJoin), ds2);

            var tds = mgr.AddSource(typeof(TerminationSource));
            wds.AddInput(tds);
            var sink = wds.Sink(typeof(TestSink), 1);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(5000);
            var result = StaticTestHelper.GetStatus();
            Assert.False(result.Item1, result.Item2);
        }
    }
}
