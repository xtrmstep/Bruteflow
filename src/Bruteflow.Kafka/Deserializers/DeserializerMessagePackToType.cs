using System;
using Confluent.Kafka;
using MessagePack;

namespace Bruteflow.Kafka.Deserializers
{
    public class DeserializerMessagePackToType<TInput> : IDeserializer<TInput>
    {
        public TInput Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var contract = data != null ? MessagePackSerializer.Deserialize<TInput>(data.ToArray()) : default;
            return contract;
        }
    }
}