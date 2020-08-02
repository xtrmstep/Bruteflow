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
        
        public void Increment(long value, string bucket)
        {
            _stats.Increment(value, bucket);
        }

        public void Gauge(double value, string bucket)
        {
            _stats.Gauge(value, bucket);
        }

        public void Timing(long duration, string bucket)
        {
            _stats.Timing(duration, bucket);
        }
    }
}