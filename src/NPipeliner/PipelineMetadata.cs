using System;

namespace NPipeliner
{
    public struct PipelineMetadata
    {
        public object Metadata { get; set; }
        public DateTime InputTimestamp { get; set; }
    }
}