using System;
using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsIncoming
{
    public class TestPipelineSettings : AbstractKafkaPipelineSettings
    {
        public TestAbstractKafkaConsumerSettings AbstractKafka { get; set; } = new TestAbstractKafkaConsumerSettings();
    }

    public class TestAbstractKafkaConsumerSettings : AbstractKafkaConsumerSettings
    {
        public TestAbstractKafkaConsumerSettings()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-incoming-events";
            var dateTime = DateTime.Now;
            GroupId = $"bruteflow-{dateTime:yyyyMMdd}-{dateTime:HH:mm:ss}";
            TestMode = true;
        }
    }
}