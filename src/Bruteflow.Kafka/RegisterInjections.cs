using System;
using Bruteflow.Kafka.Stats;
using JustEat.StatsD;
using Microsoft.Extensions.DependencyInjection;

namespace Bruteflow.Kafka
{
    public static class RegisterInjections
    {
        public static void RegisterKafkaPipelines<T>(this IServiceCollection services, Action<IServiceCollection> register = null)
        {
            services.AddSingleton<IStatsDPublisher, SilentStatsDPublisher>();
            register?.Invoke(services);
        }
    }
}