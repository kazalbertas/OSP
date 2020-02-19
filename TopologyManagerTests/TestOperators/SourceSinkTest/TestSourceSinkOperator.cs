using Orleans.TestingHost;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace OSPTests.TestOperators.SourceSinkTest
{
    [Collection(ClusterCollection.Name)]
    public class TestSourceSinkOperator
    {
        private readonly TestCluster _cluster;

        public TestSourceSinkOperator(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        //[Fact]
        //public void TestSourceSinkTopology()
        //{
        //    var conf = new TopologyConfiguration();
        //    var mgr = new TopologyManager(conf);
        //    var ds = mgr.AddSource(typeof(TestSource1));

        //    var sourceGuid = ds.OperatorGUIDs;
        //    Type sourceType = ds.OperatorType;

        //    ds.Sink(typeof(TestSink1));

        //    Assert.Equal(sourceGuid, ds.OperatorGUIDs);
        //    Assert.Equal(sourceType, ds.OperatorType);

        //    Assert.Equal(typeof(TestSink1), ds.Next.OperatorType);
        //    Assert.Null(ds.Next.Next);
        //}

        [Fact]
        public async System.Threading.Tasks.Task TestSourceSinkRunAsync() 
        {
            var breaker = _cluster.GrainFactory.GetGrain<ITestHelper>(this.GetType().Namespace);
            await breaker.Reset();
            await breaker.TempFailTest("Initial test fail");

            var conf = new TopologyConfiguration();
            var mgr = new TopologyManager(conf);
            var ds = mgr.AddSource(typeof(TestSource1));

            ds.Sink(typeof(TestSink1));
            JobManager jmgr = new JobManager();
            await jmgr.StartJob(mgr, _cluster.Client);

            Thread.Sleep(1000);
            var result = await breaker.GetStatus();
            Assert.False(result.Item1, result.Item2);
        }
    }
}
