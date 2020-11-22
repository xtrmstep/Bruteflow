namespace Bruteflow
{
    /// <summary>
    /// Interface of a block which can produces data
    /// </summary>
    /// <typeparam name="TOutput">Data type which block produces</typeparam>
    public interface IProducerBlock<out TOutput> 
    {
        /// <summary>
        ///     Link a receiving block
        /// </summary>
        /// <param name="receiverBlock"></param>
        void Link(IReceiverBlock<TOutput> receiverBlock);
    }
}