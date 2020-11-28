using System;

namespace Bruteflow
{
    /// <summary>
    /// Metadata for data entity of a pipeline
    /// </summary>
    public class PipelineMetadata
    {
        public object Metadata { get; set; }
        /// <summary>
        /// Refers to the moment when data ahs been pushed to a pipeline. It should not be changed. 
        /// </summary>
        public DateTime InputTimestamp { get; set; }
    }
}