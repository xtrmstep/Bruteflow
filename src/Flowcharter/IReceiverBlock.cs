namespace Flowcharter
{
    public interface IReceiverBlock<in TInput>
    {
        void Post(TInput input, PipelineMetadata metadata);
    }

    public interface IProducerBlock<out TOutput>
    {
        void Link(IReceiverBlock<TOutput> receiverBlock);
    }
}