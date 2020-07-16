using System;

namespace Flowcharter.Blocks
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

        public void Post(TInput input, PipelineMetadata metadata)
        {
            var condition = _condition(input, metadata);

            if (condition) _positive?.Post(input, metadata);
            else _negative?.Post(input, metadata);
        }

        void IConditionalProducerBlock<TInput, TInput>.LinkPositive(IReceiverBlock<TInput> receiverBlock)
        {
            _positive = receiverBlock;
        }

        void IConditionalProducerBlock<TInput, TInput>.LinkNegative(IReceiverBlock<TInput> receiverBlock)
        {
            _negative = receiverBlock;
        }
    }

    public static class DecisionBlockExtensions
    {
        public static void Decision<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<TPrecedingOutput, PipelineMetadata, bool> condition,
            IReceiverBlock<TPrecedingOutput> positive,
            IReceiverBlock<TPrecedingOutput> negative)
        {
            var next = new DecisionBlock<TPrecedingOutput>(condition);
            var conditional = (IConditionalProducerBlock<TPrecedingOutput, TPrecedingOutput>) next;
            conditional.LinkPositive(positive);
            conditional.LinkNegative(negative);
            precedingBlock.Link(next);
        }
    }
}