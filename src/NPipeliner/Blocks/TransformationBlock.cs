using System;

namespace NPipeliner.Blocks
{
    public class TransformationBlock<TInput, TOutput> : BroadcastingBlock<TOutput>, IReceiverBlock<TInput>
    {
        private readonly Func<TInput, PipelineMetadata, TOutput> _transformation;

        public TransformationBlock(Func<TInput, PipelineMetadata, TOutput> transformation, params IReceiverBlock<TOutput>[] next) : base(next)
        {
            _transformation = transformation;
        }

        public void Process(TInput input, PipelineMetadata metadata)
        {
            var transformed = _transformation(input, metadata);

            Broadcast(transformed, metadata);
        }
    }
}