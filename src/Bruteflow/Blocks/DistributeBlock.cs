using System;
using System.Collections.Generic;
using System.Threading;

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

        public void Push(CancellationToken cancellationToken, TEntity input, PipelineMetadata metadata)
        {
            if (_targets == null) return;

            foreach (var target in _targets) target.Push(cancellationToken, input, metadata);
        }

        public void Flush(CancellationToken cancellationToken)
        {
            foreach (var target in _targets) target.Flush(cancellationToken);
        }
    }
}