using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Consumers.Abstract
{
    internal class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>
    {
        protected IConsumer<TKey, TValue> Consumer;

        public KafkaConsumer(string topic, IConsumer<TKey, TValue> consumer)
        {
            Consumer = consumer;
            Consumer.Subscribe(topic);
        }

        public virtual ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken)
        {
            var consumeResult = Consumer.Consume(cancellationToken);
            return consumeResult;
        }

        public void Close()
        {
            Consumer.Close();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Consumer?.Dispose();
            Consumer = null;
        }
    }
}