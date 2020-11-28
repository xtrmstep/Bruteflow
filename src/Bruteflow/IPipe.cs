using Bruteflow.Blocks;

namespace Bruteflow
{
    public interface IPipe<TInput>
    {
        /// <summary>
        /// The head of the main pipeline 
        /// </summary>
        HeadBlock<TInput> Head { get; }
    }
}