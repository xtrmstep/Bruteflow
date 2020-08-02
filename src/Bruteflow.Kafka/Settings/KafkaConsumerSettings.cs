using System.Collections.Generic;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Settings
{
    public abstract class KafkaConsumerSettings
    {
        public List<string> Brokers { get; set; }
        public string Topic { get; set; }
        public string GroupId { get; set; }
        public bool TestMode { get; set; }
        public bool EnableAutoCommit => !TestMode;
        public AutoOffsetReset AutoOffsetReset => TestMode ? AutoOffsetReset.Earliest : AutoOffsetReset.Latest;
    }
}