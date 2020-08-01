using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using JustEat.StatsD;

namespace Bruteflow.Kafka.Stats
{
    public static class StatsDExtensions
    {
        public static Metrics Metric(this IStatsDPublisher stats)
        {
            return new Metrics(stats);
        }

        public struct Metrics
        {
            private readonly IStatsDPublisher _stats;

            public Metrics(IStatsDPublisher stats)
            {
                _stats = stats;
            }

            public Metrics ProduceLatency(Action action)
            {
                _stats.Time(nameof(StatMetrics.Time.ProduceLatency), action);
                return this;
            }

            /// <summary>
            ///     Counter of items which are produced to underlying stream
            /// </summary>
            /// <returns></returns>
            public Metrics ProduceCountIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Throughput.Produced));
                return this;
            }

            public Metrics CountInstances(object instance)
            {
                if (instance == null) return this;
                _stats.Increment($"instance.{GetRealTypeName(instance.GetType())}.counter");
                return this;
            }

            private static string GetRealTypeName(Type type)
            {
                if (!type.IsGenericType)
                    return type.Name;

                var sb = new StringBuilder();
                sb.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
                sb.Append('-');
                var appendComma = false;
                foreach (var arg in type.GetGenericArguments())
                {
                    if (appendComma) sb.Append('-');
                    sb.Append(GetRealTypeName(arg));
                    appendComma = true;
                }

                return sb.ToString();
            }

            public T ConsumeLatency<T>(Func<T> func)
            {
                return _stats.Time(nameof(StatMetrics.Time.ConsumeLatency), func);
            }

            /// <summary>
            ///     Counter of items consumed from outside stream
            /// </summary>
            /// <remarks>Items which are consumed may be not valid for the pipeline and will be filtered on later stages</remarks>
            /// <returns></returns>
            public Metrics ConsumedIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Throughput.Consumed));
                return this;
            }

            public Metrics SentBytes(in int sentBytes)
            {
                _stats.Gauge(sentBytes, nameof(StatMetrics.Volume.SentBytes));
                return this;
            }

            public Metrics AvailableThreads()
            {
                ThreadPool.GetAvailableThreads(out var workerThreads, out _);
                _stats.Gauge(workerThreads, nameof(StatMetrics.Volume.AvailableThreads));
                return this;
            }

            public Metrics SentBytes(JsonElement obj)
            {
                var sentBytes = Encoding.UTF8.GetBytes(obj.ToString()).Length;
                SentBytes(sentBytes);
                return this;
            }

            public Metrics PipelineLatency(PipelineMetadata metadata)
            {
                var waitTime = DateTime.Now.Subtract(metadata.InputTimestamp);
                _stats.Timing(waitTime, nameof(StatMetrics.Time.PipelineLatency));
                return this;
            }

            public Metrics WarningIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Count.Warning));
                return this;
            }

            public Metrics ErrorsIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Count.Errors));
                return this;
            }

            public Metrics FatalErrorIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Count.FatalError));
                return this;
            }
        }
    }
}