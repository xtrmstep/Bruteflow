using System.Threading;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Consumers.Abstract
{
    internal class KafkaConsumerWithMetrics<TKey, TValue> : KafkaConsumer<TKey, TValue>
    {
        protected readonly IMetricsPublisher Stats;

        public KafkaConsumerWithMetrics(string topic, IConsumer<TKey, TValue> consumer, IMetricsPublisher stats)
            : base(topic, consumer)
        {
            Stats = stats;
        }

        public override ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken)
        {
            var consumerResult = Stats.Metric().ConsumeLatency(() => Consumer.Consume(cancellationToken));
            Stats.Metric().ConsumedIncrement();
            return consumerResult;
        }
    }
}