namespace Bruteflow.Abstract
{
    public class DataItem<T>
    {
        public T Data { get; set; } 
        public PipelineMetadata Metadata { get; set; }
    }
}