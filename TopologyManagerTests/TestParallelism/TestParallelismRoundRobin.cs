using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace OSPTests.TestParallelism
{

    [Collection(ClusterCollection.Name)]
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
    public class TestParallelismRoundRobin
    {
        private readonly TestCluster _cluster;

        public TestParallelismRoundRobin(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async System.Threading.Tasks.Task TestParallelAsync()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            StaticTestHelper.Reset();
            StaticTestHelper.TempFailTest("Init test fail");
            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource), 2);
            ds.Sink(typeof(TestSink1), 2);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(1000);
            var result = StaticTestHelper.GetStatus();
            Assert.False(result.Item1,result.Item2);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestParallelMoreStreams()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            StaticTestHelper.Reset();
            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource), 3);
            ds.Sink(typeof(TestSink1), 3);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(1000);
            var result = StaticTestHelper.GetStatus();
            Assert.True(result.Item1, result.Item2);
            //must break because messages go 1,2,1,2 and there are 3 sinks 1->first 2->second 1->third 2->first = fail 
        }

        [Fact]
        public async System.Threading.Tasks.Task TestParallelFail1Async()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            StaticTestHelper.Reset();
            StaticTestHelper.TempFailTest("Init test fail");
            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1), 2);
            ds.Sink(typeof(TestSink1), 2);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(1000);
            var result = StaticTestHelper.GetStatus();
            Assert.True(result.Item1, result.Item2);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestParallelFail2Async()
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            StaticTestHelper.Reset();
            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource2), 2);
            ds.Sink(typeof(TestSink1), 2);
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);
            Thread.Sleep(1000);
            var result = StaticTestHelper.GetStatus();
            Assert.True(result.Item1, result.Item2);
        }
    }
}
