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
            TOutput output = default;            
            Parallel.Invoke(() =>
            {
                output = _process(cancellationToken, input, metadata);
            });
            Parallel.Invoke(() => _following?.Push(cancellationToken, output, metadata));
        }

        public void Flush(CancellationToken cancellationToken)
        {
            Parallel.Invoke(() => _following?.Flush(cancellationToken));
        }
    }
}