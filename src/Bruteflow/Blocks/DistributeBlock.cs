﻿using System.Collections.Generic;

namespace Bruteflow.Blocks
{
    public class DistributeBlock<TEntity> : IReceiverBlock<TEntity>, IProducerBlock<TEntity>
    {
        private readonly List<IReceiverBlock<TEntity>> _targets = new List<IReceiverBlock<TEntity>>();

        protected internal DistributeBlock()
        {
        }

        void IProducerBlock<TEntity>.Link(IReceiverBlock<TEntity> receiverBlock)
        {
            _targets.Add(receiverBlock);
        }

        public void Push(TEntity input, PipelineMetadata metadata)
        {
            if (_targets == null) return;

            foreach (var target in _targets) target.Push(input, metadata);
        }

        public void Flush()
        {
            foreach (var target in _targets) target.Flush();
        }
    }
}