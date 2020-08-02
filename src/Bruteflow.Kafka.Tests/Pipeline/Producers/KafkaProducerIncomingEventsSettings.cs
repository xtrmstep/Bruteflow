using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.Producers
{
    public class KafkaProducerIncomingEventsSettings : KafkaProducerSettings
    {
        public KafkaProducerIncomingEventsSettings()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-incoming-events";
        }
    }
}