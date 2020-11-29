using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     The block applies boolean check to incoming entity and pushes them further depends on the result.
    ///     If True, the entity is pushed to the positive branch. If False - to the negative one.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public sealed class DecisionBlock<TInput> : IReceiverBlock<TInput>, IConditionalProducerBlock<TInput, TInput>
    {
        private readonly Func<CancellationToken, TInput, PipelineMetadata, Task<bool>> _condition;
        private IReceiverBlock<TInput> _negative;
        private IReceiverBlock<TInput> _positive;

        internal DecisionBlock(Func<CancellationToken, TInput, PipelineMetadata, Task<bool>> condition)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition), "Cannot be null");
        }

        void IConditionalProducerBlock<TInput, TInput>.LinkPositive(IReceiverBlock<TInput> receiverBlock)
        {
            _positive = receiverBlock;
        }

        void IConditionalProducerBlock<TInput, TInput>.LinkNegative(IReceiverBlock<TInput> receiverBlock)
        {
            _negative = receiverBlock;
        }

        public async Task PushAsync(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            if (_positive == null && _negative == null)
            {
                throw new InvalidOperationException("Decision block should have at least one branch");
            }

            var condition = await _condition(cancellationToken, input, metadata).ConfigureAwait(false);
            var task = condition
                ? _positive?.PushAsync(cancellationToken, input, metadata)
                : _negative?.PushAsync(cancellationToken, input, metadata);

            if (task != null)
            {
                await task.ConfigureAwait(false);
            }
        }

        public async Task FlushAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                _positive?.FlushAsync(cancellationToken) ?? Task.CompletedTask,
                _negative?.FlushAsync(cancellationToken) ?? Task.CompletedTask).ConfigureAwait(false);
        }
    }
}