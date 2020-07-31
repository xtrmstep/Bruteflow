using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using JustEat.StatsD;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Consumers
{
    public abstract class AbstractConsumerWithMetricsFactory<TKey, TValue> : AbstractConsumerFactory<TKey, TValue>
    {
        private readonly IStatsDPublisher _stats;

        protected AbstractConsumerWithMetricsFactory(ILogger<AbstractConsumerWithMetricsFactory<TKey, TValue>> logger,
            KafkaConsumerSettings settings, IDeserializer<TValue> deserializer, IStatsDPublisher stats)
            : base(logger, settings, deserializer)
        {
            _stats = stats;
        }

        public override IKafkaConsumer<TKey, TValue> CreateConsumer()
        {
            var kafkaConsumer = base.CreateConsumer();

            _stats.Measure().CountInstances(kafkaConsumer);

            return kafkaConsumer;
        }

        protected override KafkaConsumer<TKey, TValue> CreateKafkaConsumer(ConsumerBuilder<TKey, TValue> consumerBuilder, string kafkaTopic)
        {
            var consumer = consumerBuilder.Build();
            var kafkaConsumer = new KafkaConsumerWithMetrics<TKey, TValue>(kafkaTopic, consumer, _stats);
            
            _stats.Measure().CountInstances(kafkaConsumer);
            
            return kafkaConsumer;
        }
    }
}