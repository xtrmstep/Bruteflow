using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            public async Task<Metrics> ProduceLatency(Func<Task> action)
            {
                var elapsed = await TimeCounter(action).ConfigureAwait(false);
                await _stats.Timing((long)elapsed.TotalMilliseconds, nameof(StatMetrics.Time.ProduceLatency)).ConfigureAwait(false);
                return this;
            }

            /// <summary>
            ///     Counter of items which are produced to underlying stream
            /// </summary>
            /// <returns></returns>
            public async Task<Metrics> ProduceCountIncrement()
            {
                await _stats.Increment(1, nameof(StatMetrics.Throughput.Produced)).ConfigureAwait(false);
                return this;
            }

            public async Task<Metrics> CountInstances(object instance)
            {
                if (instance == null) return this;
                await _stats.Increment(1, $"instance.{GetRealTypeName(instance.GetType())}.counter").ConfigureAwait(false);
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

            public async Task<T> ConsumeLatency<T>(Func<Task<T>> func)
            {
                var result = await TimeCounter(func).ConfigureAwait(false);
                await _stats.Timing((long)result.Elapsed.TotalMilliseconds, nameof(StatMetrics.Time.ConsumeLatency)).ConfigureAwait(false);
                return result.Result;
            }
            
            private static async Task<TimeCounterResult<T>> TimeCounter<T>(Func<Task<T>> func)
            {
                var sw = Stopwatch.StartNew();
                var result = await func().ConfigureAwait(false);
                sw.Stop();                
                return new TimeCounterResult<T>
                {
                    Result = result,
                    Elapsed = sw.Elapsed
                };
            }
            
            private static async Task<TimeSpan> TimeCounter(Func<Task> action)
            {
                var sw = Stopwatch.StartNew();
                await action().ConfigureAwait(false);
                sw.Stop();
                return sw.Elapsed;
            }

            /// <summary>
            ///     Counter of items consumed from outside stream
            /// </summary>
            /// <remarks>Items which are consumed may be not valid for the pipeline and will be filtered on later stages</remarks>
            /// <returns></returns>
            public async Task<Metrics> ConsumedIncrement()
            {
                await _stats.Increment(1, nameof(StatMetrics.Throughput.Consumed)).ConfigureAwait(false);
                return this;
            }

            public async Task<Metrics> SentBytes(int sentBytes)
            {
                await _stats.Gauge(sentBytes, nameof(StatMetrics.Volume.SentBytes)).ConfigureAwait(false);
                return this;
            }

            public async Task<Metrics> AvailableThreads()
            {
                ThreadPool.GetAvailableThreads(out var workerThreads, out _);
                await _stats.Gauge(workerThreads, nameof(StatMetrics.Volume.AvailableThreads)).ConfigureAwait(false);
                return this;
            }

            public async Task<Metrics> SentBytes(JObject obj)
            {
                var sentBytes = Encoding.UTF8.GetBytes(obj.ToString(Formatting.None)).Length;
                await SentBytes(sentBytes).ConfigureAwait(false);
                return this;
            }

            public async Task<Metrics> PipelineLatency(PipelineMetadata metadata)
            {
                var latency = DateTime.Now.Subtract(metadata.Timestamp);
                await _stats.Timing((long)latency.TotalMilliseconds, nameof(StatMetrics.Time.PipelineLatency)).ConfigureAwait(false);
                return this;
            }

            public async Task<Metrics> WarningIncrement()
            {
                await _stats.Increment(1, nameof(StatMetrics.Count.Warning)).ConfigureAwait(false);
                return this;
            }

            public async Task<Metrics> ErrorsIncrement()
            {
                await _stats.Increment(1, nameof(StatMetrics.Count.Errors)).ConfigureAwait(false);
                return this;
            }

            public async Task<Metrics> FatalErrorIncrement()
            {
                await _stats.Increment(1, nameof(StatMetrics.Count.FatalError)).ConfigureAwait(false);
                return this;
            }
        }
    }
}