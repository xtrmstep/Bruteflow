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

        protected override void Emit(Message<TKey, TValue> message)
        {
            _stats.Metric().ProduceLatency(() => Producer.Produce(Topic, message));
            _stats.Metric().ProduceCountIncrement();
        }
    }
}