using System;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Bruteflow.Kafka.Deserializers
{
    public sealed class ValueDeserializerToJsonElement : IDeserializer<JsonElement>
    {
        public JsonElement Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var json = Encoding.UTF8.GetString(data.ToArray());
            return JsonDocument.Parse(json).RootElement;
        }
    }
}