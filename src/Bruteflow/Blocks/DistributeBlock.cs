using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     The block which pushes entities to all following blocks synchronously
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class DistributeBlock<TEntity> : IReceiverBlock<TEntity>, IProducerBlock<TEntity> 
    {
        private readonly List<IReceiverBlock<TEntity>> _targets = new List<IReceiverBlock<TEntity>>();

        internal DistributeBlock()
        {
        }

        void IProducerBlock<TEntity>.Link(IReceiverBlock<TEntity> receiverBlock)
        {
            if (receiverBlock == null)
            {
                throw new ArgumentNullException(nameof(receiverBlock), "Cannot be null");
            }

            _targets.Add(receiverBlock);
        }

        public async Task PushAsync(CancellationToken cancellationToken, TEntity input, PipelineMetadata metadata)
        {
            if (_targets == null) return;
            var tasks = _targets.Select(target => target.PushAsync(cancellationToken, input, metadata)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task FlushAsync(CancellationToken cancellationToken)
        {
            var tasks = _targets.Select(target => target.FlushAsync(cancellationToken)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}