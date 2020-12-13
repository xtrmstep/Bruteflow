using System;

namespace Bruteflow
{
    /// <summary>
    /// Metadata for a fetched data item
    /// </summary>
    public class PipelineMetadata
    {
        /// <summary>
        /// Information which is associated with fetched data 
        /// </summary>
        public object Metadata { get; set; }
        /// <summary>
        /// Date/time when data has been fetched
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}