using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;
using JustEat.StatsD;

namespace Bruteflow.Kafka.Stats
{
    public static class StatsDExtensions
    {
        public static Measurement Measure(this IStatsDPublisher stats)
        {
            return new Measurement(stats);
        }

        public struct Measurement
        {
            private readonly IStatsDPublisher _stats;

            public Measurement(IStatsDPublisher stats)
            {
                _stats = stats;
            }

            public Measurement ProduceLatency(Action action)
            {
                _stats.Time(nameof(StatMetrics.Time.ProduceLatency), action);
                return this;
            }

            /// <summary>
            /// Counter of items which are produced to underlying stream
            /// </summary>
            /// <returns></returns>
            public Measurement ProduceCountIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Throughput.Produced));
                return this;
            }

            public Measurement CountInstances(object instance)
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
            /// Counter of items consumed from outside stream
            /// </summary>
            /// <remarks>Items which are consumed may be not valid for the pipeline and will be filtered on later stages</remarks>
            /// <returns></returns>
            public Measurement ConsumeCountIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Throughput.Consumed));
                return this;
            }

            public Measurement Payload(in int sentBytes)
            {
                _stats.Gauge(sentBytes, nameof(StatMetrics.Volume.SentBytes));
                return this;
            }

            public Measurement AvailableThreads()
            {
                ThreadPool.GetAvailableThreads(out var workerThreads, out _);
                _stats.Gauge(workerThreads, nameof(StatMetrics.Volume.AvailableThreads));
                return this;
            }

            public Measurement Payload(JsonElement obj)
            {
                var sentBytes = Encoding.UTF8.GetBytes(obj.ToString()).Length;
                Payload(sentBytes);
                return this;
            }

            public Measurement PipelineLatency(PipelineMetadata metadata)
            {
                var waitTime = DateTime.Now.Subtract(metadata.InputTimestamp);
                _stats.Timing(waitTime, nameof(StatMetrics.Time.PipelineLatency));
                return this;
            }

            public Measurement CountWarnings()
            {
                _stats.Increment(nameof(StatMetrics.Count.Warning));
                return this;
            }

            public Measurement CountErrors()
            {
                _stats.Increment(nameof(StatMetrics.Count.Errors));
                return this;
            }

            public Measurement CountCrashes()
            {
                _stats.Increment(nameof(StatMetrics.Count.FatalError));
                return this;
            }

            /// <summary>
            /// Counter of items which are valid for a pipeline
            /// </summary>
            /// <returns></returns>
            public Measurement AcceptedIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Throughput.Accepted));
                return this;
            }
            /// <summary>
            /// Counter of items which are queued for sending
            /// </summary>
            /// <returns></returns>
            public Measurement SentIncrement()
            {
                _stats.Increment(nameof(StatMetrics.Throughput.Sent));
                return this;
            }
        }
    }
}