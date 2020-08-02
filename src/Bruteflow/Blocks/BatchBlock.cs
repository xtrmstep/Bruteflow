﻿using System.Collections.Generic;

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
            _batchSize = batchSize;
        }

        void IProducerBlock<TEntity[]>.Link(IReceiverBlock<TEntity[]> receiverBlock)
        {
            _next = receiverBlock;
        }

        public void Push(TEntity input, PipelineMetadata metadata)
        {
            _latestMetadata = metadata;
            if (_delayedCount + 1 > _batchSize) SendBatchedData(metadata);

            _delayedCount++;
            _batch.Add(input);
        }

        public void Flush()
        {
            SendBatchedData(_latestMetadata);
        }

        private void SendBatchedData(PipelineMetadata metadata)
        {
            _next?.Push(_batch.ToArray(), metadata);
            _batch.Clear();
            _delayedCount = 0;
        }
    }
}