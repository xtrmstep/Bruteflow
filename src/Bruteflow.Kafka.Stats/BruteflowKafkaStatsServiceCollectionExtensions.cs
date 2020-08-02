using System;
using JustEat.StatsD;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Stats
{
    public static class BruteflowKafkaStatsServiceCollectionExtensions
    {
        public static void ConfigureStatsForPipelines<T>(this IServiceCollection services, IConfigurationRoot configuration,
            string pipeline)
        {
            services.AddSingleton<IMetricsPublisher, BruteflowStatsDPublisher>();
            services.AddStatsD(
                provider =>
                {
                    var statsDSettings = configuration.GetSection(nameof(StatsDSettings));
                    var logger = provider.GetService<ILogger<T>>();
                    return new StatsDConfiguration
                    {
                        Host = statsDSettings["HostName"],
                        Port = int.Parse(statsDSettings["Port"]),
                        Prefix = $"{statsDSettings["Prefix"]}.{pipeline.ToLowerInvariant()}",
                        OnError = ex =>
                        {
                            logger.LogError(ex, ex.Message);
                            //return true; // ignore the exception
                            return false;
                        }
                    };
                });
        }
    }
}