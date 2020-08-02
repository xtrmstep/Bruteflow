using System.Collections.Generic;

namespace Bruteflow.Kafka.Settings
{
    public class KafkaProducerSettings
    {
        public List<string> Brokers { get; } = new List<string>();
        public string Topic { get; set; }
    }
}