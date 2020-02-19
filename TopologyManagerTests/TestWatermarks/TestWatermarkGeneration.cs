using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace OSPTests.TestWatermarks
{

    [Collection(ClusterCollection.Name)]
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
    public class TestWatermarkGeneration
    {
        private readonly TestCluster _cluster;

        public TestWatermarkGeneration(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async System.Threading.Tasks.Task TestWatermark()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            await breaker.Reset();
            await breaker.TempFailTest("Init test fail");
            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource), 1);
            var sink = ds.Sink(typeof(TestSink),1);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(7000);
            var result = await breaker.GetStatus();
            Assert.False(result.Item1, result.Item2);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestWatermarkEvent()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            await breaker.Reset();
            await breaker.TempFailTest("Init test fail");
            var conf = new TopologyConfiguration();
            conf.TimeCharacteristic = CoreOSP.TimePolicy.EventTime;
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1), 1);
            var sink = ds.Sink(typeof(TestSink1), 1);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(7000);
            var result = await breaker.GetStatus();
            Assert.False(result.Item1, result.Item2);
        }

    }
}
