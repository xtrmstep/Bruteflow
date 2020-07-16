using System.Collections.Generic;

namespace Flowcharter.Blocks
{
    public class DistributeBlock<TEntity> : IReceiverBlock<TEntity>, IProducerBlock<TEntity>
    {
        private readonly List<IReceiverBlock<TEntity>> _targets = new List<IReceiverBlock<TEntity>>();

        protected internal DistributeBlock()
        {
        }

        public void Post(TEntity input, PipelineMetadata metadata)
        {
            if (_targets == null) return;

            foreach (var target in _targets) target.Post(input, metadata);
        }

        void IProducerBlock<TEntity>.Link(IReceiverBlock<TEntity> receiverBlock)
        {
            _targets.Add(receiverBlock);
        }
    }

    public static class DistributeBlockExtensions
    {
        public static void Distribute<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            params IReceiverBlock<TPrecedingOutput>[] followingBlocks)
        {
            var next = new DistributeBlock<TPrecedingOutput>();
            var producer = (IProducerBlock<TPrecedingOutput>) next;
            foreach (var followingBlock in followingBlocks)
            {
                producer.Link(followingBlock);
            }
            precedingBlock.Link(next);
        }
    }
}