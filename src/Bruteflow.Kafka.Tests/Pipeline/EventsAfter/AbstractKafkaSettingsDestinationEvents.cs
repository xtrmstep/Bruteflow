using System;
using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsAfter
{
    public class AbstractKafkaSettingsDestinationEvents : AbstractKafkaPipelineSettings
    {
        public ConsumerSettingsDestinationEvents Kafka { get; set; } = new ConsumerSettingsDestinationEvents();
    }

    public class ConsumerSettingsDestinationEvents : AbstractKafkaConsumerSettings
    {
        public ConsumerSettingsDestinationEvents()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-events-after-pipeline";
            var dateTime = DateTime.Now;
            GroupId = $"bruteflow-{dateTime:yyyyMMdd}-{dateTime:HH:mm:ss}";
            TestMode = true;
        }
    }
}