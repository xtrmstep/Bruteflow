using System;
using System.Threading;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     The block applies boolean check to incoming entity and pushes them further depends on the result.
    ///     If True, the entity is pushed to the positive branch. If False - to the negative one.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public sealed class DecisionBlock<TInput> : IReceiverBlock<TInput>, IConditionalProducerBlock<TInput, TInput>
    {
        private readonly Func<CancellationToken, TInput, PipelineMetadata, bool> _condition;
        private IReceiverBlock<TInput> _negative;
        private IReceiverBlock<TInput> _positive;

        internal DecisionBlock(Func<CancellationToken, TInput, PipelineMetadata, bool> condition)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition), "Cannot be null");
        }

        void IConditionalProducerBlock<TInput, TInput>.LinkPositive(IReceiverBlock<TInput> receiverBlock)
        {
            _positive = receiverBlock ?? throw new ArgumentNullException(nameof(receiverBlock), "Cannot be null");
        }

        void IConditionalProducerBlock<TInput, TInput>.LinkNegative(IReceiverBlock<TInput> receiverBlock)
        {
            _negative = receiverBlock ?? throw new ArgumentNullException(nameof(receiverBlock), "Cannot be null");
        }

        public void Push(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            var condition = _condition(cancellationToken, input, metadata);

            if (condition) _positive?.Push(cancellationToken, input, metadata);
            else _negative?.Push(cancellationToken, input, metadata);
        }

        public void Flush(CancellationToken cancellationToken)
        {
            _positive?.Flush(cancellationToken);
            _negative?.Flush(cancellationToken);
        }
    }
}