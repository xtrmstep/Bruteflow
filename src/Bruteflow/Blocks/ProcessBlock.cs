using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     The block which applies a transformation or other logic to incoming entity, before pushing it further
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public sealed class ProcessBlock<TInput, TOutput> : IReceiverBlock<TInput>, IProducerBlock<TOutput>
    {
        private readonly Func<CancellationToken, TInput, PipelineMetadata, Task<TOutput>> _process;
        private IReceiverBlock<TOutput> _following;

        internal ProcessBlock() : this(null)
        {
        }

        internal ProcessBlock(Func<CancellationToken, TInput, PipelineMetadata, Task<TOutput>> process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process), "Cannot be null");
        }

        void IProducerBlock<TOutput>.Link(IReceiverBlock<TOutput> receiverBlock)
        {
            _following = receiverBlock ?? throw new ArgumentNullException(nameof(receiverBlock), "Cannot be null");
        }

        public async Task PushAsync(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            var result = await _process(cancellationToken, input, metadata).ConfigureAwait(false);
            if (_following != null)
            {
                await _following.PushAsync(cancellationToken, result, metadata).ConfigureAwait(false);
            }
        }

        public async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (_following != null)
            {
                await _following.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}