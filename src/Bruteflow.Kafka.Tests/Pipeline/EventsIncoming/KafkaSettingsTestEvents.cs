using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsIncoming
{
    public class KafkaSettingsTestEvents : KafkaProducerSettings
    {
        public KafkaSettingsTestEvents()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-incoming-events";
        }
    }
}