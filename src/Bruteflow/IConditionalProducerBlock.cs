namespace Bruteflow
{
    /// <summary>
    /// Interface of a producer block with positive and negative output
    /// </summary>
    /// <typeparam name="TPositive"></typeparam>
    /// <typeparam name="TNegative"></typeparam>
    public interface IConditionalProducerBlock<out TPositive, out TNegative>
    {
        void LinkPositive(IReceiverBlock<TPositive> receiverBlock);
        void LinkNegative(IReceiverBlock<TNegative> receiverBlock);
    }
}