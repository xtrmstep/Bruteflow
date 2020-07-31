using System.Text.Json;
using Confluent.Kafka;
using JustEat.StatsD;
using Microsoft.Extensions.Logging;
using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Stats;

namespace Bruteflow.Kafka.Producers
{
    public abstract class AbstractProducerFactory<TKey, TValue> : IProducerFactory<TKey, TValue>
    {
        private static IKafkaProducer<TKey, TValue> _producerRegistrations;        
        private static readonly object _lockObject = new object();
        
        private readonly ISerializer<TKey> _keySerializer;
        private readonly ILogger<AbstractProducerFactory<TKey, TValue>> _logger;
        private readonly KafkaProducerSettings _settings;
        private readonly ISerializer<TValue> _valueSerializer;
        private readonly IStatsDPublisher _stats;

        private AbstractProducerFactory(ILogger<AbstractProducerFactory<TKey, TValue>> logger, KafkaProducerSettings settings,
            ISerializer<TKey> keySerializer, ISerializer<TValue> valueSerializer, IStatsDPublisher stats)
        {
            _logger = logger;
            _settings = settings;
            _keySerializer = keySerializer;
            _valueSerializer = valueSerializer;
            _stats = stats;
            
        }

        protected AbstractProducerFactory(ILogger<AbstractProducerFactory<TKey, TValue>> logger, KafkaProducerSettings settings, 
            ISerializer<TValue> valueSerializer, IStatsDPublisher stats)
            : this(logger, settings, null, valueSerializer, stats)
        {
        }

        public virtual IKafkaProducer<TKey, TValue> CreateProducer()
        {
            _logger.LogDebug($"Registering producer for topic '{_settings.Topic}'");
            _logger.LogTrace(JsonSerializer.Serialize(_settings));

            if (_producerRegistrations != null)
                return _producerRegistrations;

            lock (_lockObject)
            {                
                // create a new one and register in static register
                var config = new ProducerConfig
                {
                    BootstrapServers = string.Join(',', _settings.Brokers)
                };
                var producerBuilder = new ProducerBuilder<TKey, TValue>(config);

                if (_keySerializer != null) producerBuilder.SetKeySerializer(_keySerializer);
                if (_valueSerializer != null) producerBuilder.SetValueSerializer(_valueSerializer);
                var producer = producerBuilder.Build();
                var kafkaProducer = new KafkaProducer<TKey, TValue>(_logger, _settings.Topic, producer, _stats);

                _stats.Measure().CountInstances(kafkaProducer);

                _producerRegistrations ??= kafkaProducer;
            }

            return _producerRegistrations;
        }
    }
    
}