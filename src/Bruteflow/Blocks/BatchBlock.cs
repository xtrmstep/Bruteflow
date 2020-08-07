using System;
using System.Collections.Generic;
using System.Threading;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     Block which keeps a number of entities (size of batch) before pushing them to the following block
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class BatchBlock<TEntity> : IReceiverBlock<TEntity>, IProducerBlock<TEntity[]>
    {
        private readonly List<TEntity> _batch = new List<TEntity>();
        private readonly int _batchSize;
        private int _delayedCount;
        private PipelineMetadata _latestMetadata;
        private IReceiverBlock<TEntity[]> _next;

        internal BatchBlock(int batchSize)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentNullException(nameof(batchSize), "Must be greater than 0");
            }
            _batchSize = batchSize;
        }

        void IProducerBlock<TEntity[]>.Link(IReceiverBlock<TEntity[]> receiverBlock)
        {
            _next = receiverBlock ?? throw new ArgumentNullException(nameof(receiverBlock), "Cannot be null");
        }

        public void Push(CancellationToken cancellationToken, TEntity input, PipelineMetadata metadata)
        {
            _latestMetadata = metadata;
            if (_delayedCount + 1 > _batchSize) SendBatchedData(cancellationToken, metadata);

            _delayedCount++;
            _batch.Add(input);
        }

        public void Flush(CancellationToken cancellationToken)
        {
            SendBatchedData(cancellationToken, _latestMetadata);
        }

        private void SendBatchedData(CancellationToken cancellationToken, PipelineMetadata metadata)
        {
            _next?.Push(cancellationToken, _batch.ToArray(), metadata);
            _batch.Clear();
            _delayedCount = 0;
        }
    }
}