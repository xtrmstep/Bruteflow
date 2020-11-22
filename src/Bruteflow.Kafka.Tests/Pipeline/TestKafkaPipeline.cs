using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Blocks;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Producers;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline
{
    public class TestKafkaPipeline : AbstractKafkaPipeline<Ignore, JObject>
    {
        private readonly IKafkaProducer<string, JObject> _producer;

        public TestKafkaPipeline(ILogger<TestKafkaPipeline> logger,
            IConsumerFactory<Ignore, JObject> consumerFactory,
            IProducerFactory<string, JObject> producerFactory
        )
            : base(logger, consumerFactory)
        {
            _producer = producerFactory.CreateProducer();

            // pipeline definition
            Head
                .Process(AddProperty)
                .Action(Send);
        }

        private Task Send(CancellationToken cancellationToken, JObject json, PipelineMetadata metadata)
        {
            return _producer.ProduceAsync("key", json);
        }

        private static Task<JObject> AddProperty(CancellationToken cancellationToken, JObject json, PipelineMetadata metadata)
        {
            json.Add(new JProperty("testProperty", 1));
            return Task.FromResult(json);
        }
    }
}