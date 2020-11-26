using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Consumers
{
    public interface IKafkaConsumer<TKey, TValue> : IDisposable
    {
        ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken);
        void Close();
    }
}