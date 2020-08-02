namespace Bruteflow
{
    public interface IProducerBlock<out TOutput>
    {
        /// <summary>
        /// Link receiving block 
        /// </summary>
        /// <param name="receiverBlock"></param>
        void Link(IReceiverBlock<TOutput> receiverBlock);
    }
}