using System.Threading;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using JustEat.StatsD;

namespace Bruteflow.Kafka.Consumers
{
    public class KafkaConsumerWithMetrics<TKey, TValue> : KafkaConsumer<TKey, TValue>
    {
        protected readonly IStatsDPublisher Stats;

        public KafkaConsumerWithMetrics(string topic, IConsumer<TKey, TValue> consumer, IStatsDPublisher stats)
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