﻿namespace Bruteflow.Kafka.Producers
{
    public interface IProducerFactory<TKey, TValue>
    {
        IKafkaProducer<TKey, TValue> CreateProducer();
    }
}