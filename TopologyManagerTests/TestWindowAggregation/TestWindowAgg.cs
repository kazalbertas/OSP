using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace OSPTests.TestWindowAggregation
{

    [Collection(ClusterCollection.Name)]
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
    public class TestWindowAgg
    {
        private readonly TestCluster _cluster;

        public TestWindowAgg(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async System.Threading.Tasks.Task TestParallelAsync()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            await breaker.Reset();
            await breaker.TempFailTest("Init fail of the test");
            var conf = new TopologyConfiguration();
            conf.TimeCharacteristic = CoreOSP.TimePolicy.EventTime;
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource), 1);
            var wds = ds.WindowAggregate(typeof(SumWindowAgg));
            var sink = wds.Sink(typeof(TestSink),1);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(10000);
            var result = await breaker.GetStatus();
            Assert.False(result.Item1, result.Item2);
        }

    }
}
