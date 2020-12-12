using System.Collections.Generic;

namespace Bruteflow.Kafka.Settings
{
    public abstract class AbstractKafkaProducerSettings
    {
        public List<string> Brokers { get; } = new List<string>();
        public string Topic { get; set; }
    }
}