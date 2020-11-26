using System.Threading;
using System.Threading.Tasks;
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

        public override async Task<ConsumeResult<TKey, TValue>> Consume(CancellationToken cancellationToken)
        {
            var consumerResult = await Stats.Metric().ConsumeLatency(() => Task.FromResult(Consumer.Consume(cancellationToken))).ConfigureAwait(false);
            await Stats.Metric().ConsumedIncrement().ConfigureAwait(false);
            return consumerResult;
        }
    }
}