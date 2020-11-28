using System;
using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsAfter
{
    public class KafkaSettingsDestinationEvents : KafkaPipelineSettings
    {
        public KafkaSettingsDestinationEvents()
        {
            Kafka.Brokers.Add("localhost:9092");
            Kafka.Topic = "bruteflow-events-after-pipeline";
            var dateTime = DateTime.Now;
            Kafka.GroupId = $"bruteflow-{dateTime:yyyyMMdd}-{dateTime:HH:mm:ss}";
            Kafka.TestMode = true;
        }
    }
}