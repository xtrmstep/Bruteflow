using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsAfter
{
    public class TestPipelineProducerSettings : KafkaProducerSettings
    {
        public TestPipelineProducerSettings()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-events-after-pipeline";
        }
    }
}