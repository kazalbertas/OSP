using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace OSPTests.TestStringHashCodePartitioner
{

    [Collection(ClusterCollection.Name)]
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
    public class TestParallelismHash
    {
        private readonly TestCluster _cluster;

        public TestParallelismHash(ClusterFixture fixture)
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
            Assert.False(result.Item1, result.Item2);
        }


    }
}
