using GrainImplementations.Operators;
using GrainInterfaces.Operators;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.TestingHost;
using OSPJobManager;
using OSPTests.TestOperators.SourceSinkTest;
using System;
using System.Collections.Generic;
using System.Text;
using UserGrainImplementations;
using Xunit;

namespace OSPTests
{
    public class ClusterFixture : IDisposable
    {
        public ClusterFixture()
        {
            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<TestSiloConfigurations>()
                .AddClientBuilderConfigurator<TestClientConfigurations>();
            this.Cluster = builder.Build();
            this.Cluster.Deploy();
        }

        public void Dispose()
        {
            this.Cluster.StopAllSilos();
        }

        public TestCluster Cluster { get; private set; }
    }

    internal class TestClientConfigurations : IClientBuilderConfigurator
    {
        public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
        {
            clientBuilder.ConfigureApplicationParts(parts => parts
                .AddApplicationPart(typeof(TestSourceA).Assembly)
                .AddApplicationPart(typeof(Source<object>).Assembly)
                .AddApplicationPart(typeof(Job).Assembly)
                .AddApplicationPart(typeof(ISource).Assembly)
                .AddApplicationPart(typeof(TestSink1).Assembly)
                .WithReferences()
                ).AddSimpleMessageStreamProvider("SMSProvider");
        }
    }

    public class TestSiloConfigurations : ISiloBuilderConfigurator
    {
        public void Configure(ISiloHostBuilder hostBuilder)
        {
            hostBuilder
                .ConfigureApplicationParts(parts => parts
                .AddApplicationPart(typeof(TestSourceA).Assembly)
                .AddApplicationPart(typeof(Source<object>).Assembly)
                .AddApplicationPart(typeof(Job).Assembly)
                .AddApplicationPart(typeof(ISource).Assembly)
                .AddApplicationPart(typeof(TestSink1).Assembly)
                .WithReferences()
                ).AddSimpleMessageStreamProvider("SMSProvider")
                .AddMemoryGrainStorage("PubSubStore");

        }
    }

    [CollectionDefinition(ClusterCollection.Name)]
    public class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
        public const string Name = "ClusterCollection";
    }
}
