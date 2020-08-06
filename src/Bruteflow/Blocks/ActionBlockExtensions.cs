using System;
using System.Threading;

namespace Bruteflow.Blocks
{
    public static class ActionBlockExtensions
    {
        public static void Action<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Action<CancellationToken, TPrecedingOutput, PipelineMetadata> action)
        {
            var next = new ActionBlock<TPrecedingOutput>(action);
            precedingBlock.Link(next);
        }
    }
}