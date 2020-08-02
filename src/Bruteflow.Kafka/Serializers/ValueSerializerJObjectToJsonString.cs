using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Serializers
{
    public sealed class ValueSerializerJObjectToJsonString : ISerializer<JObject>
    {
        public byte[] Serialize(JObject data, SerializationContext context)
        {
            var json = data.ToString(Formatting.None);
            return Encoding.UTF8.GetBytes(json);
        }

    }
}