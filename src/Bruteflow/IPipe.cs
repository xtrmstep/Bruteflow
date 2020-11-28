using Bruteflow.Blocks;

namespace Bruteflow
{
    public interface IPipe<in TInput>
    {
        /// <summary>
        /// The head of the main pipeline 
        /// </summary>
        IReceiverBlock<TInput> Head { get; }
    }
}