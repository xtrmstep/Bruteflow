using System;
using Bruteflow.Kafka.Consumers;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline
{
    public class TestKafkaPipeline : AbstractKafkaPipeline<Ignore, JObject>
    {
        public TestKafkaPipeline(ILogger<TestKafkaPipeline> logger,
            IConsumerFactory<Ignore, JObject> consumerFactory,
            IServiceProvider serviceProvider
        )
            : base(logger, consumerFactory, serviceProvider)
        {
        }
        
        /// <inheritdoc />
        protected override IPipe<JObject> CreatePipe(IServiceProvider scopeServiceProvider)
        {
            return scopeServiceProvider.GetService<TestPipe>();
        }
    }
}