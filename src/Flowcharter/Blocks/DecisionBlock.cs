using System;
using System.Threading.Tasks;

namespace Flowcharter.Blocks
{
    public class DecisionBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Func<TInput, PipelineMetadata, bool> _condition;
        private readonly IReceiverBlock<TInput> _positive;
        private readonly IReceiverBlock<TInput> _negative;

        public DecisionBlock(Func<TInput, PipelineMetadata, bool> condition, IReceiverBlock<TInput> positive, IReceiverBlock<TInput> negative)
        {
            _condition = condition;
            _positive = positive;
            _negative = negative;
        }

        public void Process(TInput input, PipelineMetadata metadata)
        {
            var condition = _condition(input, metadata);

            if (condition) _positive?.Process(input, metadata);
            else _negative?.Process(input, metadata);
        }
    }
}