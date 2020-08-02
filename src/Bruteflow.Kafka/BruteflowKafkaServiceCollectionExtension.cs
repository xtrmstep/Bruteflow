using System;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Consumers.Abstract;
using Bruteflow.Kafka.Deserializers;
using Bruteflow.Kafka.Producers;
using Bruteflow.Kafka.Producers.Abstract;
using Bruteflow.Kafka.Serializers;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka
{
    public static class BruteflowKafkaServiceCollectionExtension
    {
        public static void AddBruteflowKafkaPipelines(this IServiceCollection services, Action<IServiceProvider, IServiceCollection> register)
        {
            services.AddSingleton<IMetricsPublisher, SilentStatsDPublisher>();
            services.AddSingleton<IDeserializer<JObject>, ValueDeserializerToJObject>();
            services.AddSingleton<ISerializer<JObject>, ValueSerializerJObjectToJsonString>();

            register?.Invoke(services.BuildServiceProvider(), services);
        }
    }
}