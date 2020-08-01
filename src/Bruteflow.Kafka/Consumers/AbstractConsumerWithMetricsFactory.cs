using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using JustEat.StatsD;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Consumers
{
    public abstract class AbstractConsumerWithMetricsFactory<TKey, TValue> : AbstractConsumerFactory<TKey, TValue>
    {
        protected readonly IStatsDPublisher Stats;

        protected AbstractConsumerWithMetricsFactory(ILogger<AbstractConsumerWithMetricsFactory<TKey, TValue>> logger,
            KafkaConsumerSettings settings, IDeserializer<TValue> valueDeserializer, IStatsDPublisher stats)
            : base(logger, settings, valueDeserializer)
        {
            Stats = stats;
        }

        public override IKafkaConsumer<TKey, TValue> CreateConsumer()
        {
            var kafkaConsumer = base.CreateConsumer();

            Stats.Metric().CountInstances(kafkaConsumer);

            return kafkaConsumer;
        }

        protected override KafkaConsumer<TKey, TValue> CreateKafkaConsumer(ConsumerBuilder<TKey, TValue> consumerBuilder, string kafkaTopic)
        {
            var consumer = consumerBuilder.Build();
            var kafkaConsumer = new KafkaConsumerWithMetrics<TKey, TValue>(kafkaTopic, consumer, Stats);
            
            Stats.Metric().CountInstances(kafkaConsumer);
            
            return kafkaConsumer;
        }
    }
}