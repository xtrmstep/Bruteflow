using System;

namespace Bruteflow.Kafka.Stats
{
    public class TimeCounterResult<T>
    {
        public T Result { get; set; }
        public TimeSpan Elapsed { get; set; }
    }
}