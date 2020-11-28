namespace Bruteflow.Abstract
{
    public class DataItem<T>
    {
        public T Entity { get; set; } 
        public PipelineMetadata Metadata { get; set; }
    }
}