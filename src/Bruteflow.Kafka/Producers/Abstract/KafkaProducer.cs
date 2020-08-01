using System;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Producers.Abstract
{
    internal class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
    {
        protected readonly ILogger Logger;
        protected readonly IProducer<TKey, TValue> Producer;
        protected readonly string Topic;

        protected internal KafkaProducer(ILogger logger, string topic, IProducer<TKey, TValue> producer)
        {
            Logger = logger;
            Topic = topic;
            Producer = producer;
        }

        public void Dispose()
        {
            Producer?.Dispose();
        }

        public void Produce(TKey key, TValue value)
        {
            try
            {
                var message = new Message<TKey, TValue> {Value = value};
                if (key != null)
                    message.Key = key;

                Emit(message);
            }
            catch (Exception err)
            {
                Logger.LogError(err, err.Message);
            }
        }

        protected virtual void Emit(Message<TKey, TValue> message)
        {
            Producer.Produce(Topic, message);
        }
    }
}