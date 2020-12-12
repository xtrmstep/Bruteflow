using System.Threading.Tasks;
using Bruteflow.Kafka.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bruteflow.Kafka.Producers.Abstract
{
    public abstract class AbstractProducerFactory<TKey, TValue> : IProducerFactory<TKey, TValue>
    {
        private readonly object _lockObject = new object();
        private volatile IKafkaProducer<TKey, TValue> _producerRegistrations;

        protected readonly ISerializer<TKey> KeySerializer;
        protected readonly ILogger<AbstractProducerFactory<TKey, TValue>> Logger;
        protected readonly AbstractKafkaProducerSettings Settings;
        protected readonly ISerializer<TValue> ValueSerializer;

        protected AbstractProducerFactory(ILogger<AbstractProducerFactory<TKey, TValue>> logger,
            AbstractKafkaProducerSettings settings, ISerializer<TKey> keySerializer, ISerializer<TValue> valueSerializer)
        {
            Logger = logger;
            Settings = settings;
            KeySerializer = keySerializer;
            ValueSerializer = valueSerializer;
        }

        protected AbstractProducerFactory(ILogger<AbstractProducerFactory<TKey, TValue>> logger,
            AbstractKafkaProducerSettings settings, ISerializer<TValue> valueSerializer)
            : this(logger, settings, null, valueSerializer)
        {
        }

        public IKafkaProducer<TKey, TValue> CreateProducer()
        {
            Logger.LogDebug($"Registering producer for topic '{Settings.Topic}'");
            Logger.LogTrace(JsonConvert.SerializeObject(Settings));

            if (_producerRegistrations != null) return _producerRegistrations;
            
            lock (_lockObject)
            {
                if (_producerRegistrations != null) return _producerRegistrations;
                
                var producerBuilder = CreateProducerBuilder();
                SetKeySerializer(producerBuilder);
                SetValueSerializer(producerBuilder);
                var producer = producerBuilder.Build();
                _producerRegistrations = CreateKafkaProducer(producer, Settings.Topic);
            }
            return _producerRegistrations;
        }

        protected virtual IKafkaProducer<TKey, TValue> CreateKafkaProducer(IProducer<TKey, TValue> producer, string kafkaTopic)
        {
            return new KafkaProducer<TKey, TValue>(Logger, kafkaTopic, producer);
        }

        protected virtual void SetValueSerializer(ProducerBuilder<TKey, TValue> producerBuilder)
        {
            if (ValueSerializer != null) producerBuilder.SetValueSerializer(ValueSerializer);
        }

        protected virtual void SetKeySerializer(ProducerBuilder<TKey, TValue> producerBuilder)
        {
            if (KeySerializer != null) producerBuilder.SetKeySerializer(KeySerializer);
        }

        protected virtual ProducerBuilder<TKey, TValue> CreateProducerBuilder()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = string.Join(',', Settings.Brokers)
            };
            var producerBuilder = new ProducerBuilder<TKey, TValue>(config);
            return producerBuilder;
        }
    }
}