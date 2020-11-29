using System;
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

        public async Task PushAsync(CancellationToken cancellationToken, TEntity input, PipelineMetadata metadata)
        {
            _latestMetadata = metadata;
            var batchedTask = Task.CompletedTask;
            if (_bufferedCount + 1 > _batchSize)
            {
                batchedTask = SendBatchedDataAsync(cancellationToken, metadata);
            }
            _bufferedCount++;
            _batch.Add(input);
            await Task.WhenAll(batchedTask, Task.CompletedTask).ConfigureAwait(false);
        }

        public async Task FlushAsync(CancellationToken cancellationToken)
        {
            await SendBatchedDataAsync(cancellationToken, _latestMetadata).ConfigureAwait(false);
        }

        private async Task SendBatchedDataAsync(CancellationToken cancellationToken, PipelineMetadata metadata)
        {
            var task = _next?.PushAsync(cancellationToken, _batch.ToImmutableArray(), metadata);
            _batch.Clear();
            _bufferedCount = 0;
            if (task != null)
            {
                await task.ConfigureAwait(false);
            }
        }
    }
}