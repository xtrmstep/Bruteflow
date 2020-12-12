using Bruteflow.Kafka.Consumers.Abstract;
using Bruteflow.Kafka.Tests.Pipeline.EventsIncoming;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsAfter
{
    public class ConsumerFactoryDestinationEvents : AbstractConsumerFactory<Ignore, JObject>
    {
        public ConsumerFactoryDestinationEvents(ILogger<ConsumerFactoryDestinationEvents> logger, 
            ConsumerSettingsDestinationEvents settings,
            IDeserializer<JObject> valueDeserializer)
            : base(logger, settings, valueDeserializer)
        {
        }
    }
}