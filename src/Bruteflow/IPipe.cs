using System;
using Bruteflow.Blocks;

namespace Bruteflow
{
    public interface IPipe<TInput> : IDisposable
    {
        /// <summary>
        /// The head of the main pipeline 
        /// </summary>
        HeadBlock<TInput> Head { get; }
    }
}