using System;

namespace Bruteflow.Kafka.Producers
{
    public interface IKafkaProducer<TKey, TValue> : IDisposable
    {
        void Produce(TKey key, TValue value);
    }
}