using System;
using System.Linq;
using Bruteflow.Abstract;
using JustEat.StatsD;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka.Stats
{
    
    public static class Registration
    {
        public static void AddBruteflowStats(this IServiceCollection services, Action<PipelineRegister> action)
        {
            var descriptor = new ServiceDescriptor(typeof(IMetricsPublisher), typeof(BruteflowStatsDPublisher), ServiceLifetime.Singleton);
            services.Replace(descriptor);
            
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

            public void StatD<TPipeline, TSettings>(TSettings settings, string pipeline, bool ignoreErrors = false)
                where TPipeline : AbstractPipeline
                where TSettings : StatsDSettings
            {
                _services.AddStatsD(
                    provider =>
                    {
                        var prefix = string.IsNullOrWhiteSpace(settings.Prefix)
                            ? string.Empty
                            : settings.Prefix.ToLowerInvariant();
                        if (prefix.Last() != '.')
                            prefix += ".";

                        var statsDConfiguration = new StatsDConfiguration
                        {
                            Host = settings.HostName,
                            Port = settings.Port,
                            Prefix = $"{prefix}{pipeline.ToLowerInvariant()}"
                        };
                        if (ignoreErrors)
                        {
                            statsDConfiguration.OnError = ex => true;
                        }
                        else
                        {
                            var logger = provider.GetService<ILogger<TPipeline>>();
                            statsDConfiguration.OnError = ex =>
                            {
                                logger.LogError(ex, ex.Message);
                                return false;
                            };
                        }
                        return statsDConfiguration;
                    });
            }
        }
    }
}