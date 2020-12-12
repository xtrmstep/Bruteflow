using System.Threading.Tasks;
using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Producers.Abstract
{
    public abstract class AbstractProducerWithMetricsFactory<TKey, TValue> : AbstractProducerFactory<TKey, TValue>
    {
        protected readonly IMetricsPublisher Stats;

        protected AbstractProducerWithMetricsFactory(ILogger<AbstractProducerWithMetricsFactory<TKey, TValue>> logger,
            AbstractKafkaProducerSettings settings, ISerializer<TKey> keySerializer, ISerializer<TValue> valueSerializer,
            IMetricsPublisher stats)
            : base(logger, settings, keySerializer, valueSerializer)
        {
            Stats = stats;
        }

        protected AbstractProducerWithMetricsFactory(ILogger<AbstractProducerWithMetricsFactory<TKey, TValue>> logger,
            AbstractKafkaProducerSettings settings, ISerializer<TValue> valueSerializer, IMetricsPublisher stats)
            : this(logger, settings, null, valueSerializer, stats)
        {
        }

        protected override IKafkaProducer<TKey, TValue> CreateKafkaProducer(IProducer<TKey, TValue> producer, string kafkaTopic)
        {
            var kafkaProducer = new KafkaProducerWithMetrics<TKey, TValue>(Logger, kafkaTopic, producer, Stats);
#pragma warning disable 4014
            Stats.Metric().CountInstances(kafkaProducer);
#pragma warning restore 4014
            return kafkaProducer;
        }
    }
}