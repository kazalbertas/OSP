using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace OSPTests.TestEventBasedWindow
{

    public class Test
    {
        public int Id { get; set; }
        public string KeyValue { get; set; }
        public int ValueForAggregation { get; set; }
        public DateTime EventTime { get; set; }
    }

    [Collection(ClusterCollection.Name)]
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
    public class TestEventBasedWindowAgg
    {
        private readonly TestCluster _cluster;

        public TestEventBasedWindowAgg(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async System.Threading.Tasks.Task TestParallelAsync()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(5);
            await breaker.ShouldBreak();
            var conf = new TopologyConfiguration();
            conf.TimeCharacteristic = CoreOSP.TimePolicy.EventTime;
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource), 1);
            var wds = ds.WindowAggregate(typeof(TestEventBasedAggregationM));

            var terminationds = mgr.AddSource(typeof(TestSource1), 1);
            wds.AddInput(terminationds);

            var sink = wds.Sink(typeof(TestSink),1);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(10000);
            var result = await breaker.GetBreaking();
            Assert.False(result);
        }

    }
}
