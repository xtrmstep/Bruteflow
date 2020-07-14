namespace NPipeliner
{
    public interface IReceiverBlock<in TInput>
    {
        void Process(TInput input, PipelineMetadata metadata);
    }
}