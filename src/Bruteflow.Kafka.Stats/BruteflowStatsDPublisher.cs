using System.Threading.Tasks;
using JustEat.StatsD;

namespace Bruteflow.Kafka.Stats
{
    public class BruteflowStatsDPublisher : IMetricsPublisher
    {
        private readonly IStatsDPublisher _stats;

        public BruteflowStatsDPublisher(IStatsDPublisher stats)
        {
            _stats = stats;
        }
        
        public Task Increment(long value, string bucket)
        {
            _stats.Increment(value, bucket);
            return Task.CompletedTask;
        }

        public Task Gauge(double value, string bucket)
        {
            _stats.Gauge(value, bucket);
            return Task.CompletedTask;
        }

        public Task Timing(long duration, string bucket)
        {
            _stats.Timing(duration, bucket);
            return Task.CompletedTask;
        }
    }
}