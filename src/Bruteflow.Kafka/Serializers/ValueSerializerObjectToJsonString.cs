using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Bruteflow.Kafka
{
    public partial class Serializers
    {
        public sealed class ValueSerializerObjectToJsonString<T> : ISerializer<T>
        {
            public byte[] Serialize(T data, SerializationContext context)
            {
                var json = JsonConvert.SerializeObject(data);
                return Encoding.UTF8.GetBytes(json);

            }
        }
    }
}