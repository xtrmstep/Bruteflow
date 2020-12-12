using Bruteflow.Kafka.Producers.Abstract;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsIncoming
{
    public class ProducerFactoryTestEvents : AbstractProducerFactory<string, JObject>
    {
        public ProducerFactoryTestEvents(ILogger<ProducerFactoryTestEvents> logger,
            ProducerSettingsTestEvents settings,
            ISerializer<JObject> valueSerializer) 
            : base(logger, settings, valueSerializer)
        {
        }
    }
}