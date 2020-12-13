using System;
using Bruteflow.Abstract;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka
{
    public static class Registration
    {
        private static bool _singletonRegistered = false;
        
        /// <summary>
        /// Register pipeline which reads JSON events
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public static void AddBruteflowKafkaPipelines(this IServiceCollection services, Action<PipelineRegister> action)
        {
            if (!_singletonRegistered)
            {
                services.AddSingleton<IMetricsPublisher, SilentStatsDPublisher>();
                services.AddSingleton<IDeserializer<JObject>, Deserializers.ValueDeserializerToJObject>();
                services.AddSingleton<ISerializer<JObject>, Serializers.ValueSerializerJObjectToJsonString>();

                _singletonRegistered = true;
            }

            var register = new PipelineRegister(services);
            action(register);
        }

        public class PipelineRegister
        {
            private readonly IServiceCollection _services;

            protected internal PipelineRegister(IServiceCollection services)
            {
                _services = services;
            }

            public void Pipeline<TPipeline, TInput, TPipe, TRoutines, TConsumeFactory, TConsumerSettings>(TConsumerSettings settings)
                where TPipeline : AbstractPipeline<TInput, TPipe>
                where TPipe : AbstractPipe<TInput>
                where TRoutines : class
                where TConsumeFactory : class, IConsumerFactory<Ignore, TInput>
                where TConsumerSettings : AbstractKafkaConsumerSettings
            {
                _services.AddScoped<TPipeline>();
                _services.AddScoped<TPipe>();
                _services.AddScoped<TRoutines>();
                _services.AddScoped<TConsumeFactory>();      
                
                _services.AddSingleton(settings);
            }
        }
    }
}