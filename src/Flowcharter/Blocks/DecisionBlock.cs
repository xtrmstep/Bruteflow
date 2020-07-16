using System;

namespace Flowcharter.Blocks
{
    public class DecisionBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Func<TInput, PipelineMetadata, bool> _condition;
        private readonly IReceiverBlock<TInput> _negative;
        private readonly IReceiverBlock<TInput> _positive;

        public DecisionBlock(Func<TInput, PipelineMetadata, bool> condition, IReceiverBlock<TInput> positive, IReceiverBlock<TInput> negative)
        {
            _condition = condition;
            _positive = positive;
            _negative = negative;
        }

        public void Post(TInput input, PipelineMetadata metadata)
        {
            var condition = _condition(input, metadata);

            if (condition) _positive?.Post(input, metadata);
            else _negative?.Post(input, metadata);
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
            var next = new DecisionBlock<TPrecedingOutput>(condition, positive, negative);
            precedingBlock.Link(next);
        }
    }
}