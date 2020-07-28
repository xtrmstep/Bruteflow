namespace Bruteflow
{
    public interface IReceiverBlock<in TInput>
    {
        void Push(TInput input, PipelineMetadata metadata);
        void Flush();
    }
}