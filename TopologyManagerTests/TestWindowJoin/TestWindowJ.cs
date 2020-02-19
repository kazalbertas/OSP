using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace OSPTests.TestWindowJoin
{

    [Collection(ClusterCollection.Name)]
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
    public class TestWindowJ
    {
        private readonly TestCluster _cluster;

        public TestWindowJ(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async System.Threading.Tasks.Task TestParallelAsync()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            await breaker.TempFailTest("Initial test failure");
            var conf = new TopologyConfiguration();
            conf.TimeCharacteristic = CoreOSP.TimePolicy.EventTime;
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource), 1);

            var ds2 = mgr.AddSource(typeof(TestSource1));

            var wds = ds.WindowJoin(typeof(WindowJoin),ds2);
            var sink = wds.Sink(typeof(TestSink),1);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(5000);
            var result = await breaker.GetStatus();
            Assert.False(result.Item1, result.Item2);
        }

    }
}
