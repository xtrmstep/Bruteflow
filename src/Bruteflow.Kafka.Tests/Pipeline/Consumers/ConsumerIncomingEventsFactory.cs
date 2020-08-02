using Bruteflow.Kafka.Consumers.Abstract;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.Consumers
{
    public class ConsumerIncomingEventsFactory : AbstractConsumerFactory<Ignore, JObject>
    {
        public ConsumerIncomingEventsFactory(ILogger<ConsumerIncomingEventsFactory> logger, 
            KafkaConsumerIncomingEventsSettings settings, 
            IDeserializer<JObject> valueDeserializer) 
            : base(logger, settings, valueDeserializer)
        {
        }
    }
}