using Bruteflow.Kafka.Producers.Abstract;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline.Producers
{
    public class ProducerEventsAfterPipelineFactory : AbstractProducerFactory<string, JObject>
    {
        public ProducerEventsAfterPipelineFactory(ILogger<ProducerEventsAfterPipelineFactory> logger, 
            KafkaProducerEventsAfterPipelineSettings settings, 
            ISerializer<string> keySerializer, ISerializer<JObject> valueSerializer) 
            : base(logger, settings, keySerializer, valueSerializer)
        {
        }

        public ProducerEventsAfterPipelineFactory(ILogger<ProducerEventsAfterPipelineFactory> logger, 
            KafkaProducerEventsAfterPipelineSettings settings,
            ISerializer<JObject> valueSerializer) 
            : base(logger, settings, valueSerializer)
        {
        }
    }
}