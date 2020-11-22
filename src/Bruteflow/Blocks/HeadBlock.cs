using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     Starting block of a pipeline
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public sealed class HeadBlock<TInput> : IHeadBlock<TInput>, IReceiverBlock<TInput>, IProducerBlock<TInput>
    {
        private readonly Func<CancellationToken, Func<CancellationToken, TInput, PipelineMetadata, Task>, Task> _process;
        private IReceiverBlock<TInput> _following;

        public HeadBlock() : this(null)
        {
        }

        public HeadBlock(Func<CancellationToken, Func<CancellationToken, TInput, PipelineMetadata, Task>, Task> process)
        {
            _process = process;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            if (_process == null)
            {
                throw new InvalidOperationException("Pipeline should be initialized with a process to use this method");
            }

            return _process(cancellationToken, _following.Push);
        }

        public Task Push(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            return _following?.Push(cancellationToken, input, metadata) ?? Task.CompletedTask;
        }

        public Task Flush(CancellationToken cancellationToken)
        {
            return _following?.Flush(cancellationToken) ?? Task.CompletedTask;
        }

        void IProducerBlock<TInput>.Link(IReceiverBlock<TInput> receiverBlock)
        {
            _following = receiverBlock;
        }
    }
}