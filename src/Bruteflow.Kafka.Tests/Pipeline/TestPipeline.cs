using System;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Tests.Pipeline.EventsIncoming;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline
{
    public class TestPipeline : AbstractKafkaPipeline<Ignore, JObject, TestPipe>
    {
        public TestPipeline(ILogger<TestPipeline> logger,
            IConsumerFactory<Ignore, JObject> consumerFactory,
            TestPipelineSettings settings,
            IServiceProvider serviceProvider
        )
            : base(logger, consumerFactory, settings, serviceProvider)
        {
        }
    }
}