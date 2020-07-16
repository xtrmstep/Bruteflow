using System.Collections.Generic;

namespace Flowcharter.Blocks
{
    public class BatchBlock<TEntity> : IReceiverBlock<TEntity>, IProducerBlock<TEntity[]>
    {
        private readonly List<TEntity> _batch = new List<TEntity>();
        private readonly int _batchSize;
        private IReceiverBlock<TEntity[]> _next;
        private int _delayedCount;

        protected internal BatchBlock(int batchSize)
        {
            _batchSize = batchSize;
        }

        public void Push(TEntity input, PipelineMetadata metadata)
        {
            if (_delayedCount + 1 > _batchSize)
            {
                _next?.Push(_batch.ToArray(), metadata);
                _batch.Clear();
                _delayedCount = 0;
            }
            else
            {
                _delayedCount++;
                _batch.Add(input);
            }
        }

        void IProducerBlock<TEntity[]>.Link(IReceiverBlock<TEntity[]> receiverBlock)
        {
            _next = receiverBlock;
        }
    }

    public static class BatchBlockExtensions
    {
        public static IProducerBlock<TPrecedingOutput[]> Batch<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            int batchSize)
        {
            var next = new BatchBlock<TPrecedingOutput>(batchSize);
            precedingBlock.Link(next);
            return next;
        }
    }
}