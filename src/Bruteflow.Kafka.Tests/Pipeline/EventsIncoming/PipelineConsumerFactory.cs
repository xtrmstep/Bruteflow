using Bruteflow.Kafka.Consumers.Abstract;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsIncoming
{
    public class PipelineConsumerFactory : AbstractConsumerFactory<Ignore, JObject>
    {
        public PipelineConsumerFactory(ILogger<PipelineConsumerFactory> logger, 
            ConsumerSettingsTestEvents settings, 
            IDeserializer<JObject> valueDeserializer) 
            : base(logger, settings, valueDeserializer)
        {
        }
    }
}