using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Consumers.Abstract;
using Bruteflow.Kafka.Deserializers;
using Bruteflow.Kafka.Producers;
using Bruteflow.Kafka.Producers.Abstract;
using Bruteflow.Kafka.Serializers;
using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Tests.Pipeline;
using Bruteflow.Kafka.Tests.Pipeline.Consumers;
using Bruteflow.Kafka.Tests.Pipeline.Producers;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Bruteflow.Kafka.Tests
{
    public class KafkaPipelineTests
    {
        [Fact]
        public void KafkaPipeline_should_consumer_and_produce_events()
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

            // configure tests infrastructure
            var cts = new CancellationTokenSource();
            // start producing of test events, in the end the cancellation token should be requested for cancellation
            var produceTestEventsTask = BeginProduceTestEvents(serviceProvider, 100);

            // start pipeline to listen events
            var pipelineExecuteTask = Task.Run(() =>
            {
                var pipeline = serviceProvider.GetService<TestKafkaPipeline>();
                pipeline.Execute(cts.Token);
            }, new CancellationTokenSource().Token);
            
            // sleep current thread and wait while others do their job
            Task.Delay(1000, cts.Token);
            //cts.Cancel();

            // wait tests producer to finish its work
            Task.WaitAll(produceTestEventsTask, pipelineExecuteTask);

            // verify that all messages consumed and produced
            var testEvent = ConsumeTestEvents(serviceProvider);
            testEvent.Count.Should().Be(100);
        }

        private static List<JObject> ConsumeTestEvents(ServiceProvider serviceProvider)
        {
            var testEvent = new List<JObject>();
            var consumerTestEvents = serviceProvider.GetService<ConsumerEventsAfterPipelineFactory>().CreateConsumer();
            var cts = new CancellationTokenSource();
            while (true)
            {
                var result = consumerTestEvents.Consume(cts.Token);
                if (result.IsPartitionEOF) break;
                testEvent.Add(result.Message.Value);
            }

            return testEvent;
        }

        private static Task BeginProduceTestEvents(ServiceProvider serviceProvider, int numberOfEvents)
        {
            var producer = serviceProvider.GetService<ProducerIncomingEventsFactory>().CreateProducer();
            var task = Task.Run(() =>
            {
                for (var i = 0; i < numberOfEvents; i++)
                {
                    producer.Produce(i.ToString(), JObject.FromObject(new TestEvent {Value = i}));
                }
            }, new CancellationTokenSource().Token);
            return task;
        }
    }
}