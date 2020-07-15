using Flowcharter.Blocks;

namespace Flowcharter
{
    public interface IReceiverBlock<in TInput>
    {
        void Process(TInput input, PipelineMetadata metadata);
    }
    
    public interface IProducerBlock<out TOutput>
    {
        void Link(IReceiverBlock<TOutput> receiverBlock);
    }
}