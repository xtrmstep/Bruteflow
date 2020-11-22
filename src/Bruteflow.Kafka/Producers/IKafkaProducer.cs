using System;
using System.Threading.Tasks;

namespace Bruteflow.Kafka.Producers
{
    public interface IKafkaProducer<TKey, TValue> : IAsyncDisposable // todo support Ignore for key
    {
        Task ProduceAsync(TKey key, TValue value);
    }
}