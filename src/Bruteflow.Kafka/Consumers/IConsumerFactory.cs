namespace Bruteflow.Kafka.Consumers
{
    public interface IConsumerFactory<TKey, TValue>
    {
        IKafkaConsumer<TKey, TValue> CreateConsumer();
    }
}