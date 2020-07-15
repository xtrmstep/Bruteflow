using System.Collections.Generic;

namespace Flowcharter.Blocks
{
    public class BatchBlock<TEntity> : IReceiverBlock<TEntity>
    {
        private readonly List<TEntity> _batch = new List<TEntity>();
        private readonly int _batchSize;
        private readonly IReceiverBlock<TEntity[]> _next;
        private int _delayedCount;

        public BatchBlock(int batchSize, IReceiverBlock<TEntity[]> next)
        {
            _batchSize = batchSize;
            _next = next;
        }

        public void Post(TEntity input, PipelineMetadata metadata)
        {
            if (_delayedCount + 1 == _batchSize)
            {
                _next?.Post(_batch.ToArray(), metadata);
                _batch.Clear();
                _delayedCount = 0;
            }
            else
            {
                _delayedCount++;
                _batch.Add(input);
            }
        }
    }
}