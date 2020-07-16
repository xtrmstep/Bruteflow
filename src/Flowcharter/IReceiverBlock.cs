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
    
    public interface IConditionalProducerBlock<out TPositive, out TNegative>
    {
        void LinkPositive(IReceiverBlock<TPositive> receiverBlock);
        void LinkNegative(IReceiverBlock<TNegative> receiverBlock);
    }
}