using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Kafka.Tests.Pipeline;
using Bruteflow.Kafka.Tests.Pipeline.EventsAfter;
using Bruteflow.Kafka.Tests.Pipeline.EventsIncoming;
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
            
            // classes to generate of test events
            services.AddTransient(svc => Mock.Of<ILogger<ProducerFactoryTestEvents>>());
            services.AddTransient<ProducerFactoryTestEvents>();
            services.AddTransient<ProducerSettingsTestEvents>();
            
            // classes to read processed events (from destination topic)
            services.AddTransient(svc => Mock.Of<ILogger<ConsumerFactoryProcessedEvents>>());
            services.AddTransient<ConsumerFactoryProcessedEvents>();
            services.AddTransient<ConsumerSettingsProcessedEvents>();

            // classes of the pipeline
            services.AddTransient(svc => Mock.Of<ILogger<TestPipeline>>());
            services.AddTransient(svc => Mock.Of<ILogger<PipelineConsumerFactory>>());
            services.AddTransient(svc => Mock.Of<ILogger<PipelineProducerFactory>>());            
            services.AddTransient<PipelineProducerFactory>();            
            services.AddTransient<ProducerSettingsProcessedEvents>();            
            
            services.AddBruteflowKafkaPipelines(o =>
            {
                var settings = new ConsumerSettingsTestEvents();
                o.Pipeline<TestPipeline, JObject, TestPipe, TestRoutines, PipelineConsumerFactory, ConsumerSettingsTestEvents>(settings);
            });            
            var serviceProvider = services.BuildServiceProvider();

            // produce test events
            await ProduceTestEvents(serviceProvider, 100);

            // start pipeline to listen events
            var pipeline = serviceProvider.GetService<TestPipeline>();
            // wait 10 seconds to make sure all sent event could be read
            // if less, then less message could be read and the test will fail
            await pipeline.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token); 

            // verify that all messages consumed and produced
            var testEvent = ConsumeTestEvents(serviceProvider);
            testEvent.Count.Should().Be(100);
        }

        private static List<JObject> ConsumeTestEvents(ServiceProvider serviceProvider)
        {
            var testEvent = new List<JObject>();
            var consumerTestEvents = serviceProvider.GetService<ConsumerFactoryProcessedEvents>().CreateConsumer();
            var cts = new CancellationTokenSource();
            while (true)
            {
                var result = consumerTestEvents.Consume(cts.Token);
                if (result.IsPartitionEOF) break;
                testEvent.Add(result.Message.Value);
            }

            return testEvent;
        }

        private static Task ProduceTestEvents(ServiceProvider serviceProvider, int numberOfEvents)
        {
            var producer = serviceProvider.GetService<ProducerFactoryTestEvents>().CreateProducer();
            var tasks = Enumerable.Range(0, numberOfEvents)
                .Select(i => producer.ProduceAsync(i.ToString(), JObject.FromObject(new TestEvent {Value = i})))
                .ToArray();
            return Task.WhenAll(tasks);
        }
    }
}