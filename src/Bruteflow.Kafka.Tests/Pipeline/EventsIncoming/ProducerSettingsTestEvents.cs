using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsIncoming
{
    public class ProducerSettingsTestEvents : AbstractKafkaProducerSettings
    {
        public ProducerSettingsTestEvents()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-incoming-events";
        }
    }
}