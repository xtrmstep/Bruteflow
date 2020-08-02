using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Kafka.Consumers.Abstract;
using Bruteflow.Kafka.Deserializers;
using Bruteflow.Kafka.Producers.Abstract;
using Bruteflow.Kafka.Serializers;
using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Tests.Pipeline;
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
            services.AddBruteflowKafkaPipelines();
            services.AddTransient<KafkaConsumerSettings, TestKafkaConsumerSettings>();
            services.AddTransient<KafkaProducerSettings, TestKafkaProducerSettings>();
            services.AddTransient(svc => Mock.Of<ILogger<TestKafkaPipeline>>());
            services.AddTransient(svc => Mock.Of<ILogger<AbstractConsumerFactory<Ignore, JObject>>>());
            services.AddTransient(svc => Mock.Of<ILogger<AbstractProducerFactory<string, JObject>>>());
            // logger
            services.AddTransient<TestKafkaPipeline>();
            var serviceProvider = services.BuildServiceProvider();

            // configure tests infrastructure
            var cts = new CancellationTokenSource();
            // start producing of test events, in the end the cancellation token should be requested for cancellation
            var produceTestEventsTask = BeginProduceTestEvents(100);

            // start pipeline to listen events
            var pipelineExecuteTask = Task.Run(() =>
            {
                var pipeline = serviceProvider.GetService<TestKafkaPipeline>();
                pipeline.Execute(cts.Token);
            }, new CancellationTokenSource().Token);
            
            // sleep current thread and wait while others do their job
            Task.Delay(1000, cts.Token);
            cts.Cancel();

            // wait tests producer to finish its work
            Task.WaitAll(produceTestEventsTask, pipelineExecuteTask);

            // verify that all messages consumed and produced
            var testEvent = ConsumeTestEvents();
            testEvent.Count.Should().Be(100);
        }

        private static List<JObject> ConsumeTestEvents()
        {
            var testEvent = new List<JObject>();
            var dateTime = DateTime.Now;
            var consumerTestEvents = new AbstractConsumerFactory<Ignore, JObject>(
                    Mock.Of<ILogger<AbstractConsumerFactory<Ignore, JObject>>>(),
                    new KafkaConsumerSettings
                    {
                        Brokers = {"localhost:9092"},
                        Topic = "bruteflow-events-after-pipeline",
                        GroupId = $"bruteflow-{dateTime:yyyyMMdd}-{dateTime:HH:mm:ss}",
                        TestMode = true // force to read the topic from the very beginning 
                    },
                    new ValueDeserializerToJObject()
                )
                .CreateConsumer();
            var cts = new CancellationTokenSource();
            while (true)
            {
                var result = consumerTestEvents.Consume(cts.Token);
                if (result.IsPartitionEOF) break;
                testEvent.Add(result.Message.Value);
            }

            return testEvent;
        }

        private static Task BeginProduceTestEvents(int numberOfEvents)
        {
            var producer = new AbstractProducerFactory<string, JObject>(
                    Mock.Of<ILogger<AbstractProducerFactory<string, JObject>>>(),
                    new KafkaProducerSettings
                    {
                        Brokers = {"localhost:9092"},
                        Topic = "bruteflow-incoming-events"
                    },
                    new ValueSerializerJObjectToJsonString()
                )
                .CreateProducer();
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