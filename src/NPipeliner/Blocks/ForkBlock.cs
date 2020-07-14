using System;

namespace NPipeliner.Blocks
{
    public class ForkBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Func<TInput, PipelineMetadata, bool> _conditionLeft;
        private readonly Func<TInput, PipelineMetadata, bool> _conditionRight;
        private readonly IReceiverBlock<TInput> _left;
        private readonly IReceiverBlock<TInput> _right;

        public ForkBlock(Func<TInput, PipelineMetadata, bool> conditionLeft,
            Func<TInput, PipelineMetadata, bool> conditionRight,
            IReceiverBlock<TInput> left,
            IReceiverBlock<TInput> right)
        {
            _conditionLeft = conditionLeft;
            _conditionRight = conditionRight;
            _left = left;
            _right = right;
        }

        public void Process(TInput input, PipelineMetadata metadata)
        {
            if (_conditionLeft(input, metadata)) _left.Process(input, metadata);
            if (_conditionRight(input, metadata)) _right.Process(input, metadata);
        }
    }
}