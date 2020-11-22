using System.Collections.Immutable;

namespace Bruteflow.Blocks
{
    public static class BatchBlockExtensions
    {
        public static IProducerBlock<ImmutableArray<TPrecedingOutput>> Batch<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            int batchSize)           
        {
            var next = new BatchBlock<TPrecedingOutput>(batchSize);
            precedingBlock.Link(next);
            return next;
        }
    }
}