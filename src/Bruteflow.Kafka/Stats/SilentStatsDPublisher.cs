using System.Threading.Tasks;

namespace Bruteflow.Kafka.Stats
{
    public class SilentStatsDPublisher : IMetricsPublisher
    {
        public Task Increment(long value, string bucket)
        {
            // do nothing
            return Task.CompletedTask;
        }

        public Task Gauge(double value, string bucket)
        {
            // do nothing
            return Task.CompletedTask;
        }

        public Task Timing(long duration, string bucket)
        {
            // do nothing
            return Task.CompletedTask;
        }
    }
}