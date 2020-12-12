using System;
using Bruteflow.Kafka.Settings;

namespace Bruteflow.Kafka.Tests.Pipeline.EventsIncoming
{
    public class ConsumerSettingsTestEvents : AbstractKafkaConsumerSettings
    {
        public ConsumerSettingsTestEvents()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-incoming-events";
            var dateTime = DateTime.Now;
            GroupId = $"bruteflow-{dateTime:yyyyMMdd}-{dateTime:HH:mm:ss}";
            TestMode = true;
        }
    }
}