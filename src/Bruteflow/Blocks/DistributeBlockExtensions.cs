namespace Bruteflow.Blocks
{
    public static class DistributeBlockExtensions
    {
        public static void Distribute<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            params IReceiverBlock<TPrecedingOutput>[] followingBlocks)
        {
            var next = new DistributeBlock<TPrecedingOutput>();
            var producer = (IProducerBlock<TPrecedingOutput>) next;
            foreach (var followingBlock in followingBlocks) producer.Link(followingBlock);
            precedingBlock.Link(next);
        }
    }
}