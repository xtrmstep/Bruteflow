#pragma warning disable 649

namespace Bruteflow.Kafka.Stats
{
    internal class StatMetrics
    {
        public class Volume
        {
            public object SentBytes;
            public object AvailableThreads;
        }

        public class Throughput
        {
            public object Accepted;
            public object Sent;
            public object Consumed;
            public object Produced;
        }

        public class Time
        {
            public object PipelineLatency;
            public object ProduceLatency;
            public object ConsumeLatency;
        }

        public class Count
        {
            public object Errors;
            public object Warning;
            public object FatalError;
        }
    }
}