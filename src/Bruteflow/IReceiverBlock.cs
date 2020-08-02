namespace Bruteflow
{
    public interface IReceiverBlock<in TInput>
    {
        // Push an entity to this block 
        void Push(TInput input, PipelineMetadata metadata);

        /// <summary>
        ///     Push internal state to following blocks and flush the state
        /// </summary>
        void Flush();
    }
}