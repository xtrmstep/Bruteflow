using System;
using System.Threading;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     Starting block of a pipeline
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public sealed class HeadBlock<TInput> : IHeadBlock<TInput>, IReceiverBlock<TInput>, IProducerBlock<TInput>
    {
        private readonly Action<Action<CancellationToken, TInput, PipelineMetadata>> _process;
        private IReceiverBlock<TInput> _following;

        public HeadBlock() : this(null)
        {
        }

        public HeadBlock(Action<Action<CancellationToken, TInput, PipelineMetadata>> process)
        {
            _process = process;
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (_process == null)
            {
                throw new InvalidOperationException("Pipeline should be initialized with a process to use this method");
            }
            _process(_following.Push);
        }

        public void Push(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            _following?.Push(cancellationToken, input, metadata);
        }

        public void Flush(CancellationToken cancellationToken)
        {
            _following?.Flush(cancellationToken);
        }

        void IProducerBlock<TInput>.Link(IReceiverBlock<TInput> receiverBlock)
        {
            _following = receiverBlock;
        }
    }
}