using System.Threading;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using JustEat.StatsD;

namespace Bruteflow.Kafka.Consumers
{
    public class KafkaConsumerWithMetrics<TKey, TValue> : KafkaConsumer<TKey, TValue>
    {
        private readonly IStatsDPublisher _stats;

        public KafkaConsumerWithMetrics(string topic, IConsumer<TKey, TValue> consumer, IStatsDPublisher stats)
            : base(topic, consumer)
        {
            _stats = stats;
        }

        public override ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken)
        {
            var consumerResult = _stats.Measure().ConsumeLatency(() => Consumer.Consume(cancellationToken));
            _stats.Measure().ConsumeCountIncrement();
            return consumerResult;
        }
    }
}