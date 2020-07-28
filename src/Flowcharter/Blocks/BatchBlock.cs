using System.Collections.Generic;

namespace Flowcharter.Blocks
{
    public class BatchBlock<TEntity> : IReceiverBlock<TEntity>, IProducerBlock<TEntity[]>
    {
        private readonly List<TEntity> _batch = new List<TEntity>();
        private PipelineMetadata _latestMetadata;
        private readonly int _batchSize;
        private IReceiverBlock<TEntity[]> _next;
        private int _delayedCount;

        protected internal BatchBlock(int batchSize)
        {
            _batchSize = batchSize;
        }

        public void Push(TEntity input, PipelineMetadata metadata)
        {
            _latestMetadata = metadata;
            if (_delayedCount + 1 > _batchSize)
            {
                SendBatchedData(metadata);
            }

            _delayedCount++;
            _batch.Add(input);
        }

        private void SendBatchedData(PipelineMetadata metadata)
        {
            _next?.Push(_batch.ToArray(), metadata);
            _batch.Clear();
            _delayedCount = 0;
        }

        public void Flush()
        {
            SendBatchedData(_latestMetadata);
        }

        void IProducerBlock<TEntity[]>.Link(IReceiverBlock<TEntity[]> receiverBlock)
        {
            _next = receiverBlock;
        }
    }
}