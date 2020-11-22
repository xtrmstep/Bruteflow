using System.Threading.Tasks;

namespace Bruteflow.Kafka.Stats
{
    public interface IMetricsPublisher
    {
        /// <summary>
        /// Publishes a counter for the specified bucket and value.
        /// </summary>
        /// <param name="value">The value to increment the counter by.</param>
        /// <param name="bucket">The bucket to increment the counter for.</param>
        Task Increment(long value, string bucket);

        /// <summary>
        /// Publishes a gauge for the specified bucket and value.
        /// </summary>
        /// <param name="value">The value to publish for the gauge.</param>
        /// <param name="bucket">The bucket to publish the gauge for.</param>
        Task Gauge(double value, string bucket);

        /// <summary>
        /// Publishes a timer for the specified bucket and value.
        /// </summary>
        /// <param name="duration">The value to publish for the timer.</param>
        /// <param name="bucket">The bucket to publish the timer for.</param>
        Task Timing(long duration, string bucket);
    }
}