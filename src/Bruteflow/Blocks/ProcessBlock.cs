using System;
using System.Threading;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     The block which applies a transformation or other logic to incoming entity, before pushing it further
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public sealed class ProcessBlock<TInput, TOutput> : IReceiverBlock<TInput>, IProducerBlock<TOutput>
    {
        private readonly Func<CancellationToken, TInput, PipelineMetadata, TOutput> _process;
        private IReceiverBlock<TOutput> _following;

        internal ProcessBlock() : this(null)
        {
        }

        internal ProcessBlock(Func<CancellationToken, TInput, PipelineMetadata, TOutput> process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process), "Cannot be null");
        }

        void IProducerBlock<TOutput>.Link(IReceiverBlock<TOutput> receiverBlock)
        {
            _following = receiverBlock ?? throw new ArgumentNullException(nameof(receiverBlock), "Cannot be null");
        }

        public void Push(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            var output = _process(cancellationToken, input, metadata);

            _following?.Push(cancellationToken, output, metadata);
        }

        public void Flush(CancellationToken cancellationToken)
        {
            _following?.Flush(cancellationToken);
        }
    }
}