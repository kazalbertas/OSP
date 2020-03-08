using Confluent.Kafka;
using CoreOSP.Models;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GrainImplementations.Operators
{
    public abstract class SourceKafkaProvider<T> : SourceKafkaProvider<T, T>
    {
        public override T Map(T input)
        {
            return input;
        }
    }

    public abstract class SourceKafkaProvider<T,K> : Source<T,K>
    {
        public override async Task Start()
        {
            Subscibe();
        }

        private async Task Subscibe() 
        {
            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "localhost:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe(GetTopicName());

                CancellationTokenSource cts = new CancellationTokenSource();

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            var processedMsg = ProcessMessage(cr.Value);
                            if (Filter(processedMsg))
                            {
                                K item = Map(processedMsg);
                                Data<K> dt = new Data<K>(GetKey(item), item);
                                await SendMessageToStream(dt);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }

        }

        public abstract Guid GetStreamID();
        public abstract string GetTopicName();

    }
}
