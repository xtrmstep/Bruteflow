using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Kafka.Producers;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline
{
    public class TestRoutines
    {
        private readonly IKafkaProducer<string, JObject> _producer;
        
        public TestRoutines(IProducerFactory<string, JObject> producerFactory)
        {
            _producer = producerFactory.CreateProducer();
        }
        
        public Task<JObject> AddProperty(CancellationToken cancellationToken, JObject json, PipelineMetadata metadata)
        {
            json.Add(new JProperty("testProperty", 1));
            return Task.FromResult(json);
        }
        
        public Task Send(CancellationToken cancellationToken, JObject json, PipelineMetadata metadata)
        {
            return _producer.ProduceAsync("key", json);
        }
    }
}