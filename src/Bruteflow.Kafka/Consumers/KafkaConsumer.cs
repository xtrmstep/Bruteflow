using System.Threading;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Consumers
{
    public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>
    {
        protected readonly IConsumer<TKey, TValue> Consumer;

        public KafkaConsumer(string topic, IConsumer<TKey, TValue> consumer)
        {
            Consumer = consumer;
            Consumer.Subscribe(topic);
        }

        public virtual ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken)
        {
            return Consumer.Consume(cancellationToken);
        }

        public void Close()
        {
            Consumer.Close();
        }

        public void Dispose()
        {
            Consumer?.Dispose();
        }
    }
}