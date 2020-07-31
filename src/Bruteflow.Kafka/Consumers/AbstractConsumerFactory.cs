using System.Text.Json;
using Bruteflow.Kafka.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Consumers
{
    public abstract class AbstractConsumerFactory<TKey, TValue> : IConsumerFactory<TKey, TValue>
    {
        private readonly IDeserializer<TValue> _deserializer;
        private readonly ILogger<AbstractConsumerFactory<TKey, TValue>> _logger;
        private readonly KafkaConsumerSettings _settings;

        protected AbstractConsumerFactory(ILogger<AbstractConsumerFactory<TKey, TValue>> logger,
            KafkaConsumerSettings settings, IDeserializer<TValue> deserializer)
        {
            _logger = logger;
            _settings = settings;
            _deserializer = deserializer;
        }

        public virtual IKafkaConsumer<TKey, TValue> CreateConsumer()
        {
            _logger.LogDebug($"Registering consumer {_settings.GroupId}");
            _logger.LogTrace(JsonSerializer.Serialize(_settings));

            var config = CreateConsumerConfig(_settings);
            var consumerBuilder = new ConsumerBuilder<TKey, TValue>(config);
            SetValueDeserializer(consumerBuilder);
            var kafkaConsumer = CreateKafkaConsumer(consumerBuilder, _settings.Topic);

            return kafkaConsumer;
        }

        protected virtual void SetValueDeserializer(ConsumerBuilder<TKey, TValue> consumerBuilder)
        {
            consumerBuilder.SetValueDeserializer(_deserializer);
        }

        protected virtual KafkaConsumer<TKey, TValue> CreateKafkaConsumer(ConsumerBuilder<TKey, TValue> consumerBuilder, string kafkaTopic)
        {
            var consumer = consumerBuilder.Build();
            return new KafkaConsumer<TKey, TValue>(kafkaTopic, consumer);
        }

        protected virtual ConsumerConfig CreateConsumerConfig(KafkaConsumerSettings settings)
        {
            var conf = new ConsumerConfig
            {
                BootstrapServers = string.Join(',', settings.Brokers),
                GroupId = settings.GroupId,
                EnableAutoCommit = settings.EnableAutoCommit,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = settings.AutoOffsetReset,
                EnablePartitionEof = true
            };
            return conf;
        }
    }
}