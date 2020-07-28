using System;

namespace Bruteflow.Blocks
{
    public static class ActionBlockExtensions
    {
        public static void Action<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Action<TPrecedingOutput, PipelineMetadata> action)
        {
            var next = new ActionBlock<TPrecedingOutput>(action);
            precedingBlock.Link(next);
        }
    }
}