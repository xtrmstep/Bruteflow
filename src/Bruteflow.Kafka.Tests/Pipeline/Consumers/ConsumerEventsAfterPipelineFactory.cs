using Bruteflow.Kafka.Consumers.Abstract;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.Consumers
{
    public class ConsumerEventsAfterPipelineFactory : AbstractConsumerFactory<Ignore, JObject>
    {
        public ConsumerEventsAfterPipelineFactory(ILogger<ConsumerEventsAfterPipelineFactory> logger, 
            KafkaConsumerEventsAfterPipelineSettings settings,
            IDeserializer<JObject> valueDeserializer)
            : base(logger, settings, valueDeserializer)
        {
        }
    }
}