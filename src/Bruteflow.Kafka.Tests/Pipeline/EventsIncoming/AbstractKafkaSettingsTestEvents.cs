using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsIncoming
{
    public class AbstractKafkaSettingsTestEvents : AbstractKafkaProducerSettings
    {
        public AbstractKafkaSettingsTestEvents()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-incoming-events";
        }
    }
}