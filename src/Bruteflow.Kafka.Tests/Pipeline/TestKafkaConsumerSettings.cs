using System;
using Bruteflow.Kafka.Settings;
using FluentAssertions;

namespace Bruteflow.Kafka.Tests.Pipeline
{
    class TestKafkaConsumerSettings : KafkaConsumerSettings
    {
        public TestKafkaConsumerSettings()
        {
            Brokers.Add("localhost:9092");
            Topic = "bruteflow-incoming-events";
            var dateTime = DateTime.Now;
            GroupId = $"bruteflow-{dateTime:yyyyMMdd}-{dateTime:HH:mm:ss}";
        }
    }
}