using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsAfter
{
    public class ProducerSettingsProcessedEvents : AbstractKafkaProducerSettings
    {
        public ProducerSettingsProcessedEvents()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-events-after-pipeline";
        }
    }
}