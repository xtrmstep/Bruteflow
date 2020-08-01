using JustEat.StatsD;

namespace Bruteflow.Kafka.Stats
{
    public class SilentStatsDPublisher : IStatsDPublisher
    {
        public void Increment(long value, double sampleRate, string bucket)
        {
            // do nothing
        }

        public void Gauge(double value, string bucket)
        {
            // do nothing
        }

        public void Timing(long duration, double sampleRate, string bucket)
        {
            // do nothing
        }
    }
}