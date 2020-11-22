using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    public static class ProcessBlockExtensions
    {
        /// <summary>
        /// Attach processing block which has input and output
        /// </summary>
        /// <param name="precedingBlock"></param>
        /// <param name="process">Routine which processes incoming data and returns transformed result</param>
        /// <typeparam name="TPrecedingOutput"></typeparam>
        /// <typeparam name="TCurrentOutput"></typeparam>
        /// <returns></returns>
        public static IProducerBlock<TCurrentOutput> Process<TPrecedingOutput, TCurrentOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<CancellationToken, TPrecedingOutput, PipelineMetadata, Task<TCurrentOutput>> process)
        {
            var next = new ProcessBlock<TPrecedingOutput, TCurrentOutput>(process);
            precedingBlock.Link(next);
            return next;
        }
    }
}