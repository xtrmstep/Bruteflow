namespace Flowcharter
{
    public interface IConditionalProducerBlock<out TPositive, out TNegative>
    {
        void LinkPositive(IReceiverBlock<TPositive> receiverBlock);
        void LinkNegative(IReceiverBlock<TNegative> receiverBlock);
    }
}