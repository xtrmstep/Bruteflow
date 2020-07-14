using System;

namespace NPipeliner.Blocks
{
    public class ConditionBlock<TInput> : BroadcastingBlock<TInput>, IReceiverBlock<TInput>
    {
        private readonly Func<TInput, PipelineMetadata, bool> _condition;

        public ConditionBlock(Func<TInput, PipelineMetadata, bool> condition, params IReceiverBlock<TInput>[] next) : base(next)
        {
            _condition = condition;
        }

        public void Process(TInput input, PipelineMetadata metadata)
        {
            var condition = _condition(input, metadata);

            if (condition) Broadcast(input, metadata);
        }
    }
}