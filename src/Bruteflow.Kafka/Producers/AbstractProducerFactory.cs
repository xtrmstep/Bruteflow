using System.Text.Json;
using Bruteflow.Kafka.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Producers
{
    public abstract class AbstractProducerFactory<TKey, TValue> : IProducerFactory<TKey, TValue>
    {
        private static readonly object _lockObject = new object();

        private static IKafkaProducer<TKey, TValue> _producerRegistrations;
        private readonly ISerializer<TKey> _keySerializer;
        private readonly KafkaProducerSettings _settings;
        private readonly ISerializer<TValue> _valueSerializer;

        protected readonly ILogger<AbstractProducerFactory<TKey, TValue>> Logger;

        protected AbstractProducerFactory(ILogger<AbstractProducerFactory<TKey, TValue>> logger,
            KafkaProducerSettings settings, ISerializer<TKey> keySerializer, ISerializer<TValue> valueSerializer)
        {
            Logger = logger;
            _settings = settings;
            _keySerializer = keySerializer;
            _valueSerializer = valueSerializer;
        }

        protected AbstractProducerFactory(ILogger<AbstractProducerFactory<TKey, TValue>> logger,
            KafkaProducerSettings settings, ISerializer<TValue> valueSerializer)
            : this(logger, settings, null, valueSerializer)
        {
        }

        public IKafkaProducer<TKey, TValue> CreateProducer()
        {
            Logger.LogDebug($"Registering producer for topic '{_settings.Topic}'");
            Logger.LogTrace(JsonSerializer.Serialize(_settings));

            if (_producerRegistrations == null) return _producerRegistrations;
            lock (_lockObject)
            {
                if (_producerRegistrations == null) return _producerRegistrations;

                var producerBuilder = CreateProducerBuilder();
                SetKeySerializer(producerBuilder);
                SetValueSerializer(producerBuilder);
                var producer = producerBuilder.Build();
                _producerRegistrations = CreateKafkaProducer(producer, _settings.Topic);
                return _producerRegistrations;
            }
        }

        protected virtual KafkaProducer<TKey, TValue> CreateKafkaProducer(IProducer<TKey, TValue> producer, string kafkaTopic)
        {
            return new KafkaProducer<TKey, TValue>(Logger, kafkaTopic, producer);
        }

        protected virtual void SetValueSerializer(ProducerBuilder<TKey, TValue> producerBuilder)
        {
            if (_valueSerializer != null) producerBuilder.SetValueSerializer(_valueSerializer);
        }

        protected virtual void SetKeySerializer(ProducerBuilder<TKey, TValue> producerBuilder)
        {
            if (_keySerializer != null) producerBuilder.SetKeySerializer(_keySerializer);
        }

        protected virtual ProducerBuilder<TKey, TValue> CreateProducerBuilder()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = string.Join(',', _settings.Brokers)
            };
            var producerBuilder = new ProducerBuilder<TKey, TValue>(config);
            return producerBuilder;
        }
    }
}