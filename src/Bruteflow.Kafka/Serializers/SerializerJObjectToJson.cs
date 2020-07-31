using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Serializers
{
    public class SerializerJObjectToJson : ISerializer<JsonElement>
    {
        public byte[] Serialize(JsonElement data, SerializationContext context)
        {
            var json = data.ToString();
            return Encoding.UTF8.GetBytes(json);
        }
    }
}