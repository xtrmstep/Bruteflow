using Bruteflow.Kafka.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bruteflow.Kafka.Consumers.Abstract
{
    public abstract class AbstractConsumerFactory<TKey, TValue> : IConsumerFactory<TKey, TValue>
    {
        protected readonly ILogger<AbstractConsumerFactory<TKey, TValue>> Logger;
        protected readonly AbstractKafkaConsumerSettings Settings;
        protected readonly IDeserializer<TValue> ValueDeserializer;

        protected AbstractConsumerFactory(ILogger<AbstractConsumerFactory<TKey, TValue>> logger,
            AbstractKafkaConsumerSettings settings,
            IDeserializer<TValue> valueDeserializer)
        {
            Logger = logger;
            Settings = settings;
            ValueDeserializer = valueDeserializer;
        }

        public virtual IKafkaConsumer<TKey, TValue> CreateConsumer()
        {
            Logger.LogDebug($"Registering consumer {Settings.GroupId}");
            Logger.LogTrace(JsonConvert.SerializeObject(Settings));

            var config = CreateConsumerConfig(Settings);
            var consumerBuilder = new ConsumerBuilder<TKey, TValue>(config);
            SetValueDeserializer(consumerBuilder);
            var kafkaConsumer = CreateKafkaConsumer(consumerBuilder, Settings.Topic);

            return kafkaConsumer;
        }

        protected virtual void SetValueDeserializer(ConsumerBuilder<TKey, TValue> consumerBuilder)
        {
            consumerBuilder.SetValueDeserializer(ValueDeserializer);
        }

        protected virtual IKafkaConsumer<TKey, TValue> CreateKafkaConsumer(ConsumerBuilder<TKey, TValue> consumerBuilder, string kafkaTopic)
        {
            var consumer = consumerBuilder.Build();
            return new KafkaConsumer<TKey, TValue>(kafkaTopic, consumer);
        }

        protected virtual ConsumerConfig CreateConsumerConfig(AbstractKafkaConsumerSettings settings)
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