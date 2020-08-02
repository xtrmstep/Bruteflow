using System;
using Bruteflow.Kafka.Stats;
using Microsoft.Extensions.DependencyInjection;

namespace Bruteflow.Kafka
{
    public static class BruteflowKafkaServiceCollectionExtension
    {
        public static void AddBruteflowKafkaPipelines<T>(this IServiceCollection services, Action<IServiceCollection> register = null)
        {
            services.AddSingleton<IMetricsPublisher, SilentStatsDPublisher>();
            register?.Invoke(services);
        }
    }
}