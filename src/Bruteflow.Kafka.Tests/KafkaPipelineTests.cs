using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Producers;
using Bruteflow.Kafka.Tests.Pipeline;
using Bruteflow.Kafka.Tests.Pipeline.Consumers;
using Bruteflow.Kafka.Tests.Pipeline.Producers;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Bruteflow.Kafka.Tests
{
    public class KafkaPipelineTests
    {
        [Fact]
        public async Task KafkaPipeline_should_consumer_and_produce_events()
        {
            // configure dependencies
            IServiceCollection services = new ServiceCollection();
            services.AddBruteflowKafkaPipelines((provider, collection) =>
            {
                collection.AddTransient<KafkaConsumerIncomingEventsSettings>();
                collection.AddTransient<KafkaConsumerEventsAfterPipelineSettings>();
                collection.AddTransient<KafkaProducerIncomingEventsSettings>();
                collection.AddTransient<KafkaProducerEventsAfterPipelineSettings>();
                
                collection.AddTransient(svc => Mock.Of<ILogger<TestKafkaPipeline>>());
                collection.AddTransient(svc => Mock.Of<ILogger<ConsumerIncomingEventsFactory>>());
                collection.AddTransient(svc => Mock.Of<ILogger<ConsumerEventsAfterPipelineFactory>>());
                collection.AddTransient(svc => Mock.Of<ILogger<ProducerIncomingEventsFactory>>());
                collection.AddTransient(svc => Mock.Of<ILogger<ProducerEventsAfterPipelineFactory>>());

                collection.AddTransient(typeof(IConsumerFactory<Ignore, JObject>), typeof(ConsumerIncomingEventsFactory));
                collection.AddTransient(typeof(IProducerFactory<string, JObject>), typeof(ProducerEventsAfterPipelineFactory));
                collection.AddTransient<ProducerIncomingEventsFactory>();
                collection.AddTransient<ConsumerEventsAfterPipelineFactory>();
                
                // pipeline
                collection.AddTransient<TestKafkaPipeline>();
            });            
            var serviceProvider = services.BuildServiceProvider();

            // produce test events
            await ProduceTestEvents(serviceProvider, 100);

            // start pipeline to listen events
            var pipeline = serviceProvider.GetService<TestKafkaPipeline>();
            // wait 10 seconds to make sure all sent event could be read
            // if less, then less message could be read and the test will fail
            await pipeline.Execute(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token); 

            // verify that all messages consumed and produced
            var testEvent = await ConsumeTestEvents(serviceProvider);
            testEvent.Count.Should().Be(100);
        }

        private static async Task<List<JObject>> ConsumeTestEvents(ServiceProvider serviceProvider)
        {
            var testEvent = new List<JObject>();
            var consumerTestEvents = serviceProvider.GetService<ConsumerEventsAfterPipelineFactory>().CreateConsumer();
            var cts = new CancellationTokenSource();
            while (true)
            {
                var result = await consumerTestEvents.Consume(cts.Token);
                if (result.IsPartitionEOF) break;
                testEvent.Add(result.Message.Value);
            }

            return testEvent;
        }

        private static Task ProduceTestEvents(ServiceProvider serviceProvider, int numberOfEvents)
        {
            var producer = serviceProvider.GetService<ProducerIncomingEventsFactory>().CreateProducer();
            var tasks = Enumerable.Range(0, numberOfEvents)
                .Select(i => producer.ProduceAsync(i.ToString(), JObject.FromObject(new TestEvent {Value = i})))
                .ToArray();
            return Task.WhenAll(tasks);
        }
    }
}