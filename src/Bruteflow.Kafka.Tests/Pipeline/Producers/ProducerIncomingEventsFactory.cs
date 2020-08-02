using Bruteflow.Kafka.Producers.Abstract;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.Producers
{
    public class ProducerIncomingEventsFactory : AbstractProducerFactory<string, JObject>
    {
        public ProducerIncomingEventsFactory(ILogger<ProducerIncomingEventsFactory> logger,
            KafkaProducerIncomingEventsSettings settings, 
            ISerializer<string> keySerializer, ISerializer<JObject> valueSerializer)
            : base(logger, settings, keySerializer, valueSerializer)
        {
        }

        public ProducerIncomingEventsFactory(ILogger<ProducerIncomingEventsFactory> logger,
            KafkaProducerIncomingEventsSettings settings,
            ISerializer<JObject> valueSerializer) 
            : base(logger, settings, valueSerializer)
        {
        }
    }
}