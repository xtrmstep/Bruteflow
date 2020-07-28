namespace Flowcharter
{
    public interface IHeadBlock<in TInput>
    {
        void Start();
        void Push(TInput input, PipelineMetadata metadata);
        void Flush();
    }
}