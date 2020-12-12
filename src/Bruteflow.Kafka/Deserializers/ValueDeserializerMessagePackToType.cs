using System;
using Confluent.Kafka;
using MessagePack;

namespace Bruteflow.Kafka
{
    public sealed partial class Deserializers
    {
        public sealed class ValueDeserializerMessagePackToType<TInput> : IDeserializer<TInput>
        {
            public TInput Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                var contract = data != null ? MessagePackSerializer.Deserialize<TInput>(data.ToArray()) : default;
                return contract;
            }
        }
    }
}