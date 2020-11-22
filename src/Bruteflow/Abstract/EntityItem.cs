namespace Bruteflow.Abstract
{
    public class EntityItem<T>
    {
        public T Entity { get; set; } 
        public PipelineMetadata Metadata { get; set; }
    }
}