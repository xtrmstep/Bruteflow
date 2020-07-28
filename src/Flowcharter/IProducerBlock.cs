namespace Flowcharter
{
    public interface IProducerBlock<out TOutput>
    {
        void Link(IReceiverBlock<TOutput> receiverBlock);
    }
}