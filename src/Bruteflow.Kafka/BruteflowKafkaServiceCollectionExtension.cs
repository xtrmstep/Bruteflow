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
        public static void AddBruteflowKafkaPipelines(this IServiceCollection services)
        {
            RegisterCommonDependencies(services);

            services.AddTransient(typeof(IConsumerFactory<,>), typeof(AbstractConsumerFactory<,>));
            services.AddTransient(typeof(IProducerFactory<,>), typeof(AbstractProducerFactory<,>));
        }

        public static void AddBruteflowKafkaPipelinesWithMetrics(this IServiceCollection services)
        {
            RegisterCommonDependencies(services);
            
            services.AddTransient(typeof(IConsumerFactory<,>), typeof(AbstractConsumerFactory<,>));
            services.AddTransient(typeof(IProducerFactory<,>), typeof(AbstractProducerFactory<,>));
        }

        private static void RegisterCommonDependencies(IServiceCollection services)
        {
            services.AddSingleton<IMetricsPublisher, SilentStatsDPublisher>();
            services.AddSingleton<IDeserializer<JObject>, ValueDeserializerToJObject>();
            services.AddSingleton<ISerializer<JObject>, ValueSerializerJObjectToJsonString>();
        }
    }
}