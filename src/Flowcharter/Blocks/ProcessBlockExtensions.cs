using System;

namespace Flowcharter.Blocks
{
    public static class ProcessBlockExtensions
    {
        public static IProducerBlock<TCurrentOutput> Process<TPrecedingOutput, TCurrentOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<TPrecedingOutput, PipelineMetadata, TCurrentOutput> process)
        {
            var next = new ProcessBlock<TPrecedingOutput, TCurrentOutput>(process);
            precedingBlock.Link(next);
            return next;
        }
    }
}