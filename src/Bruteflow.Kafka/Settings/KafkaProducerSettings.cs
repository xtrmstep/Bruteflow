using System.Collections.Generic;

namespace Bruteflow.Kafka.Settings
{
    public abstract class KafkaProducerSettings
    {
        public List<string> Brokers { get; set; }
        public string Topic { get; set; }
    }
}