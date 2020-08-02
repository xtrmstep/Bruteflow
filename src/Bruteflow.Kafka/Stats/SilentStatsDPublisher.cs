namespace Bruteflow.Kafka.Stats
{
    public class SilentStatsDPublisher : IMetricsPublisher
    {
        public void Increment(long value, string bucket)
        {
            // do nothing
        }

        public void Gauge(double value, string bucket)
        {
            // do nothing
        }

        public void Timing(long duration, string bucket)
        {
            // do nothing
        }
    }
}