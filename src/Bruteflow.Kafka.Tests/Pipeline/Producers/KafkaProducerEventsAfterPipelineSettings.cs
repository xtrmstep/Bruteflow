using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.Producers
{
    public class KafkaProducerEventsAfterPipelineSettings : KafkaProducerSettings
    {
        public KafkaProducerEventsAfterPipelineSettings()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-events-after-pipeline";
        }
    }
}