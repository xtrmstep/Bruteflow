using Bruteflow.Kafka.Consumers.Abstract;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsAfter
{
    public class ConsumerFactoryDestinationEvents : AbstractConsumerFactory<Ignore, JObject>
    {
        public ConsumerFactoryDestinationEvents(ILogger<ConsumerFactoryDestinationEvents> logger, 
            KafkaSettingsDestinationEvents settings,
            IDeserializer<JObject> valueDeserializer)
            : base(logger, settings, valueDeserializer)
        {
        }
    }
}