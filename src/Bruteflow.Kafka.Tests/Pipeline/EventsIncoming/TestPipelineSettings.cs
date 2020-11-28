using System;
using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsIncoming
{
    public class TestPipelineSettings : KafkaPipelineSettings
    {
        public TestPipelineSettings()
        {
            Kafka.Brokers.Add("localhost:9092");
            Kafka.Topic = "bruteflow-incoming-events";
            var dateTime = DateTime.Now;
            Kafka.GroupId = $"bruteflow-{dateTime:yyyyMMdd}-{dateTime:HH:mm:ss}";
            Kafka.TestMode = true;
        }
    }
}