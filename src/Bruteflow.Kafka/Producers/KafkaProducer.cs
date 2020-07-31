using System;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using JustEat.StatsD;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Producers
{
    public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
    {
        private readonly ILogger _logger;
        private readonly IProducer<TKey, TValue> _producer;
        private readonly IStatsDPublisher _stats;
        private readonly string _topic;

        protected internal KafkaProducer(ILogger logger, string topic, IProducer<TKey, TValue> producer, IStatsDPublisher stats)
        {
            _logger = logger;
            _topic = topic;
            _producer = producer;
            _stats = stats;
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        public void Produce(TKey key, TValue value)
        {
            try
            {
                var message = new Message<TKey, TValue> {Value = value};
                if (key != null)
                    message.Key = key;

                _stats.Measure().ProduceLatency(() => _producer.Produce(_topic, message));
                _stats.Measure().ProduceCountIncrement();
            }
            catch (Exception err)
            {
                _logger.LogError(err, err.Message);
            }
        }
    }
}