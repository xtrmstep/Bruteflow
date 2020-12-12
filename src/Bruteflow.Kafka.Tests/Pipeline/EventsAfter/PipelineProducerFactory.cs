using Bruteflow.Kafka.Producers.Abstract;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsAfter
{
    public class PipelineProducerFactory : AbstractProducerFactory<string, JObject>
    {
        public PipelineProducerFactory(ILogger<PipelineProducerFactory> logger, 
            ProducerSettingsProcessedEvents settings,
            ISerializer<JObject> valueSerializer) 
            : base(logger, settings, valueSerializer)
        {
        }
    }
}