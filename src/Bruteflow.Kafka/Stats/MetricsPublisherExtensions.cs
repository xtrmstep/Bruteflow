using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Stats
{
    public static class MetricsPublisherExtensions
    {
        public static Metrics Metric(this IMetricsPublisher stats)
        {
            return new Metrics(stats);
        }

        public struct Metrics
        {
            private readonly IMetricsPublisher _stats;

            public Metrics(IMetricsPublisher stats)
            {
                _stats = stats;
            }

            public Metrics ProduceLatency(Action action)
            {
                var elapsed = TimeCounter(action);
                _stats.Timing((long)elapsed.TotalMilliseconds, nameof(StatMetrics.Time.ProduceLatency));
                return this;
            }

            /// <summary>
            ///     Counter of items which are produced to underlying stream
            /// </summary>
            /// <returns></returns>
            public Metrics ProduceCountIncrement()
            {
                _stats.Increment(1, nameof(StatMetrics.Throughput.Produced));
                return this;
            }

            public Metrics CountInstances(object instance)
            {
                if (instance == null) return this;
                _stats.Increment(1, $"instance.{GetRealTypeName(instance.GetType())}.counter");
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
                var result = TimeCounter(func, out var elapsed);
                _stats.Timing((long)elapsed.TotalMilliseconds, nameof(StatMetrics.Time.ConsumeLatency));
                return result;
            }

            private static T TimeCounter<T>(Func<T> func, out TimeSpan elapsed)
            {
                var sw = Stopwatch.StartNew();
                var result = func();
                sw.Stop();
                elapsed = sw.Elapsed;
                return result;
            }
            
            private static TimeSpan TimeCounter(Action action)
            {
                var sw = Stopwatch.StartNew();
                action();
                sw.Stop();
                return sw.Elapsed;
            }

            /// <summary>
            ///     Counter of items consumed from outside stream
            /// </summary>
            /// <remarks>Items which are consumed may be not valid for the pipeline and will be filtered on later stages</remarks>
            /// <returns></returns>
            public Metrics ConsumedIncrement()
            {
                _stats.Increment(1, nameof(StatMetrics.Throughput.Consumed));
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

            public Metrics SentBytes(JObject obj)
            {
                var sentBytes = Encoding.UTF8.GetBytes(obj.ToString(Formatting.None)).Length;
                SentBytes(sentBytes);
                return this;
            }

            public Metrics PipelineLatency(PipelineMetadata metadata)
            {
                var latency = DateTime.Now.Subtract(metadata.InputTimestamp);
                _stats.Timing((long)latency.TotalMilliseconds, nameof(StatMetrics.Time.PipelineLatency));
                return this;
            }

            public Metrics WarningIncrement()
            {
                _stats.Increment(1, nameof(StatMetrics.Count.Warning));
                return this;
            }

            public Metrics ErrorsIncrement()
            {
                _stats.Increment(1, nameof(StatMetrics.Count.Errors));
                return this;
            }

            public Metrics FatalErrorIncrement()
            {
                _stats.Increment(1, nameof(StatMetrics.Count.FatalError));
                return this;
            }
        }
    }
}