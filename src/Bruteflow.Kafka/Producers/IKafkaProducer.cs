using System;

namespace Bruteflow.Kafka.Producers
{
    public interface IKafkaProducer<TKey, TValue> : IDisposable // todo support Ignore for key
    {
        void Produce(TKey key, TValue value);
    }
}