using System;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka
{
    public sealed partial class Deserializers
    {
        public sealed class ValueDeserializerToJObject : IDeserializer<JObject>
        {
            public JObject Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                var json = Encoding.UTF8.GetString(data.ToArray());
                return JObject.Parse(json);
            }
        }
    }
}