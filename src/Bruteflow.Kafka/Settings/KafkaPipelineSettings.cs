namespace Bruteflow.Kafka.Settings
{
    public class KafkaPipelineSettings : PipelineSettings
    {
        public KafkaSettings Kafka { get; set; } = new KafkaSettings();

        public class KafkaSettings : KafkaConsumerSettings
        {
        }
    }
}