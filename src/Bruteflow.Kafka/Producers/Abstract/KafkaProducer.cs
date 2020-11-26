using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Producers.Abstract
{
    internal class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
    {
        protected readonly ILogger Logger;
        protected IProducer<TKey, TValue> Producer;
        protected readonly string Topic;

        protected internal KafkaProducer(ILogger logger, string topic, IProducer<TKey, TValue> producer)
        {
            if (typeof(TKey) == typeof(Ignore)) // todo support Ignore for key
            {
                throw new TypeInitializationException("Ignore is not supported", null);
            }
            
            Logger = logger;
            Topic = topic;
            Producer = producer;
        }

        public async Task ProduceAsync(TKey key, TValue value)
        {
            try
            {
                var message = new Message<TKey, TValue> {Value = value};
                if (key != null)
                    message.Key = key;

                await Emit(message).ConfigureAwait(false);
            }
            catch (Exception err)
            {
                Logger.LogError(err, err.Message);
            }
        }

        protected virtual Task Emit(Message<TKey, TValue> message)
        {
            return Producer.ProduceAsync(Topic, message);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Producer?.Dispose();
            Producer = null;
            return new ValueTask(Task.CompletedTask);
        }
    }
}