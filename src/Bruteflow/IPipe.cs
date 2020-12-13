using System;
using Bruteflow.Blocks;

namespace Bruteflow
{
    /// <summary>
    /// The interface of chained blocks and its input 
    /// </summary>
    /// <typeparam name="TInput">Data type which will be pushed to the input</typeparam>
    public interface IPipe<TInput> : IDisposable
    {
        /// <summary>
        /// The first block in the pipe which is its input 
        /// </summary>
        HeadBlock<TInput> Head { get; }
    }
}