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

            public Task<Metrics> ProduceLatency(Func<Task> action)
            {
                var that = this;
                var stats = _stats;
                return TimeCounter(action)
                    .ContinueWith(antecedent => stats.Timing((long) antecedent.Result.TotalMilliseconds, nameof(StatMetrics.Time.ProduceLatency)), TaskContinuationOptions.NotOnRanToCompletion)
                    .ContinueWith(antecedent => that, TaskContinuationOptions.NotOnRanToCompletion);
            }

            /// <summary>
            ///     Counter of items which are produced to underlying stream
            /// </summary>
            /// <returns></returns>
            public Task<Metrics> ProduceCountIncrement()
            {
                var that = this;
                return _stats.Increment(1, nameof(StatMetrics.Throughput.Produced))
                    .ContinueWith(antecedent => that, TaskContinuationOptions.NotOnRanToCompletion);
            }

            public Task<Metrics> CountInstances(object instance)
            {
                var that = this;
                if (instance == null) return Task.FromResult(this);
                return _stats.Increment(1, $"instance.{GetRealTypeName(instance.GetType())}.counter")
                    .ContinueWith(antecedent => that, TaskContinuationOptions.NotOnRanToCompletion);
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

            public Task<T> ConsumeLatency<T>(Func<Task<T>> func)
            {
                var stats = _stats;
                return TimeCounter(func)
                    .ContinueWith(antecedent =>
                        {
                            Task.Factory.StartNew(() =>
                            {
                                stats.Timing((long) antecedent.Result.Elapsed.TotalMilliseconds, nameof(StatMetrics.Time.ConsumeLatency));
                            }, TaskCreationOptions.AttachedToParent);
                            return antecedent.Result.Result;
                        },
                        TaskContinuationOptions.NotOnRanToCompletion)
                    .ContinueWith(antecedent => antecedent.Result, TaskContinuationOptions.NotOnRanToCompletion);
            }
            
            private static Task<TimeCounterResult<T>> TimeCounter<T>(Func<Task<T>> func)
            {
                var sw = Stopwatch.StartNew();
                return func().ContinueWith(antecedent =>
                {
                    sw.Stop();
                    return new TimeCounterResult<T>
                    {
                        Result = antecedent.Result,
                        Elapsed = sw.Elapsed
                    };
                });
            }
            
            private static Task<TimeSpan> TimeCounter(Func<Task> action)
            {
                var sw = Stopwatch.StartNew();
                return action()
                    .ContinueWith(antecedent =>
                    {
                        sw.Stop();
                        return sw.Elapsed;
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }

            /// <summary>
            ///     Counter of items consumed from outside stream
            /// </summary>
            /// <remarks>Items which are consumed may be not valid for the pipeline and will be filtered on later stages</remarks>
            /// <returns></returns>
            public Task<Metrics> ConsumedIncrement()
            {
                var that = this;
                return _stats.Increment(1, nameof(StatMetrics.Throughput.Consumed))
                    .ContinueWith(_ => that);
            }

            public Task<Metrics> SentBytes(int sentBytes)
            {
                var that = this;
                return _stats.Gauge(sentBytes, nameof(StatMetrics.Volume.SentBytes))
                    .ContinueWith(_ => that);
            }

            public Task<Metrics> AvailableThreads()
            {
                var that = this;
                ThreadPool.GetAvailableThreads(out var workerThreads, out _);
                return _stats.Gauge(workerThreads, nameof(StatMetrics.Volume.AvailableThreads))
                    .ContinueWith(_ => that);
            }

            public Task<Metrics> SentBytes(JObject obj)
            {
                var that = this;
                var sentBytes = Encoding.UTF8.GetBytes(obj.ToString(Formatting.None)).Length;
                return SentBytes(sentBytes)
                    .ContinueWith(_ => that);
            }

            public Task<Metrics> PipelineLatency(PipelineMetadata metadata)
            {
                var that = this;
                var latency = DateTime.Now.Subtract(metadata.InputTimestamp);
                return _stats.Timing((long) latency.TotalMilliseconds, nameof(StatMetrics.Time.PipelineLatency))
                    .ContinueWith(_ => that);
            }

            public Task<Metrics> WarningIncrement()
            {
                var that = this;
                return _stats.Increment(1, nameof(StatMetrics.Count.Warning))
                    .ContinueWith(_ => that);
            }

            public Task<Metrics> ErrorsIncrement()
            {
                var that = this;
                return _stats.Increment(1, nameof(StatMetrics.Count.Errors))
                    .ContinueWith(_ => that);
            }

            public Task<Metrics> FatalErrorIncrement()
            {
                var that = this;
                return _stats.Increment(1, nameof(StatMetrics.Count.FatalError))
                    .ContinueWith(_ => that);
            }
        }
    }
}