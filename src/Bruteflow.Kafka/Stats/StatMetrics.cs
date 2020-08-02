namespace Bruteflow.Kafka.Stats
{
    internal struct StatMetrics
    {
        public struct Volume
        {
            public const object AvailableThreads = null;
            public const object SentBytes = null;
        }

        public struct Throughput
        {
            public const object Consumed = null;
            public const object Produced = null;
        }

        public struct Time
        {
            public const object ConsumeLatency = null;
            public const object PipelineLatency = null;
            public const object ProduceLatency = null;
        }

        public struct Count
        {
            public const object Errors = null;
            public const object FatalError = null;
            public const object Warning = null;
        }
    }
}