using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using JustEat.StatsD;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Producers
{
    public abstract class AbstractProducerWithMetricsFactory<TKey, TValue> : AbstractProducerFactory<TKey, TValue>
    {
        private readonly IStatsDPublisher _stats;

        protected AbstractProducerWithMetricsFactory(ILogger<AbstractProducerWithMetricsFactory<TKey, TValue>> logger,
            KafkaProducerSettings settings, ISerializer<TKey> keySerializer, ISerializer<TValue> valueSerializer,
            IStatsDPublisher stats)
            : base(logger, settings, keySerializer, valueSerializer)
        {
            _stats = stats;
        }

        protected AbstractProducerWithMetricsFactory(ILogger<AbstractProducerWithMetricsFactory<TKey, TValue>> logger,
            KafkaProducerSettings settings, ISerializer<TValue> valueSerializer, IStatsDPublisher stats)
            : this(logger, settings, null, valueSerializer, stats)
        {
        }

        protected override KafkaProducer<TKey, TValue> CreateKafkaProducer(IProducer<TKey, TValue> producer, string kafkaTopic)
        {
            var kafkaProducer = new KafkaProducerWithMetrics<TKey, TValue>(Logger, kafkaTopic, producer, _stats);
            _stats.Measure().CountInstances(kafkaProducer);
            return kafkaProducer;
        }
    }
}