using System;

namespace Bruteflow.Blocks
{
    public class DecisionBlock<TInput> : IReceiverBlock<TInput>, IConditionalProducerBlock<TInput, TInput>
    {
        private readonly Func<TInput, PipelineMetadata, bool> _condition;
        private IReceiverBlock<TInput> _negative;
        private IReceiverBlock<TInput> _positive;

        protected internal DecisionBlock(Func<TInput, PipelineMetadata, bool> condition)
        {
            _condition = condition;
        }

        void IConditionalProducerBlock<TInput, TInput>.LinkPositive(IReceiverBlock<TInput> receiverBlock)
        {
            _positive = receiverBlock;
        }

        void IConditionalProducerBlock<TInput, TInput>.LinkNegative(IReceiverBlock<TInput> receiverBlock)
        {
            _negative = receiverBlock;
        }

        public void Push(TInput input, PipelineMetadata metadata)
        {
            var condition = _condition(input, metadata);

            if (condition) _positive?.Push(input, metadata);
            else _negative?.Push(input, metadata);
        }

        public void Flush()
        {
            _positive?.Flush();
            _negative?.Flush();
        }
    }
}