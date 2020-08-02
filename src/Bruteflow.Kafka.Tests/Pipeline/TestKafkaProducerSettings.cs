using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline
{
    class TestKafkaProducerSettings : KafkaProducerSettings
    {
        public TestKafkaProducerSettings()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-events-after-pipeline";
        }
    }
}