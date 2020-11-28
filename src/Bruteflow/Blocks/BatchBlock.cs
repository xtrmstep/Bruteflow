﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     Block which keeps a number of entities (size of batch) before pushing them to the following block
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class BatchBlock<TEntity> : IReceiverBlock<TEntity>, IProducerBlock<ImmutableArray<TEntity>>
    {
        private readonly List<TEntity> _batch = new List<TEntity>();
        private readonly int _batchSize;
        private int _bufferedCount;
        private PipelineMetadata _latestMetadata;
        private IReceiverBlock<ImmutableArray<TEntity>> _next;

        internal BatchBlock(int batchSize)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentNullException(nameof(batchSize), "Must be greater than 0");
            }
            _batchSize = batchSize;
        }

        void IProducerBlock<ImmutableArray<TEntity>>.Link(IReceiverBlock<ImmutableArray<TEntity>> receiverBlock)
        {
            _next = receiverBlock ?? throw new ArgumentNullException(nameof(receiverBlock), "Cannot be null");
        }

        public Task PushAsync(CancellationToken cancellationToken, TEntity input, PipelineMetadata metadata)
        {
            _latestMetadata = metadata;
            var batchedTask = Task.CompletedTask;
            if (_bufferedCount + 1 > _batchSize)
            {
                batchedTask = SendBatchedData(cancellationToken, metadata);
            }
            _bufferedCount++;
            _batch.Add(input);
            return Task.WhenAll(batchedTask, Task.CompletedTask);
        }

        public Task FlushAsync(CancellationToken cancellationToken)
        {
            return SendBatchedData(cancellationToken, _latestMetadata);
        }

        private Task SendBatchedData(CancellationToken cancellationToken, PipelineMetadata metadata)
        {
            var task = _next?.PushAsync(cancellationToken, _batch.ToImmutableArray(), metadata);
            _batch.Clear();
            _bufferedCount = 0;
            return task;
        }
    }
}