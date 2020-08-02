using System;
using Bruteflow.Kafka.Stats;
using Microsoft.Extensions.DependencyInjection;

namespace Bruteflow.Kafka
{
    public static class BruteflowKafkaServiceCollectionExtension
    {
        public static void AddBruteflowKafkaPipelines<T>(this IServiceCollection services)
        {
            services.AddSingleton<IMetricsPublisher, SilentStatsDPublisher>();
        }
    }
}