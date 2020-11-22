using System.Threading.Tasks;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Producers.Abstract
{
    internal class KafkaProducerWithMetrics<TKey, TValue> : KafkaProducer<TKey, TValue>
    {
        private readonly IMetricsPublisher _stats;

        protected internal KafkaProducerWithMetrics(ILogger logger, string topic, IProducer<TKey, TValue> producer,
            IMetricsPublisher stats)
            : base(logger, topic, producer)
        {
            _stats = stats;
        }

        protected override async Task Emit(Message<TKey, TValue> message)
        {
            await _stats.Metric().ProduceLatency(() => Producer.ProduceAsync(Topic, message));
            await _stats.Metric().ProduceCountIncrement();
        }
    }
}