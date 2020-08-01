using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Serializers
{
    public class SerializerObjectToJsonString<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            var json = JsonSerializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}