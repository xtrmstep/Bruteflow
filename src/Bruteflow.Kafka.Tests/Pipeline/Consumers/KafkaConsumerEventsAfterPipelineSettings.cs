using System;
using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.Consumers
{
    public class KafkaConsumerEventsAfterPipelineSettings : KafkaConsumerSettings
    {
        public KafkaConsumerEventsAfterPipelineSettings()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-events-after-pipeline";
            var dateTime = DateTime.Now;
            GroupId = $"bruteflow-{dateTime:yyyyMMdd}-{dateTime:HH:mm:ss}";
            TestMode = true;
        }
    }
}