using System;
using System.Threading.Tasks;

namespace Flowcharter.Blocks
{
    public class DistributeBlock<TEntity> : IReceiverBlock<TEntity>
    {
        private readonly IReceiverBlock<TEntity>[] _targets;

        public DistributeBlock(params IReceiverBlock<TEntity>[] targets)
        {
            _targets = targets;
        }

        public void Post(TEntity input, PipelineMetadata metadata)
        {
            if (_targets == null) return;

            foreach (var target in _targets)
            {
                target.Post(input, metadata);
            }
        }
    }

    public static class DistributeBlockExtensions
    {
        public static void Distribute<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            params IReceiverBlock<TPrecedingOutput>[] followingBlocks)
        {
            var next = new DistributeBlock<TPrecedingOutput>(followingBlocks);
            precedingBlock.Link(next);
        }
    }
}