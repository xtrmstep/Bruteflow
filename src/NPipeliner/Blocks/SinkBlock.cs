using System;

namespace NPipeliner.Blocks
{
    public class SinkBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Action<TInput, PipelineMetadata> _action;

        public SinkBlock(Action<TInput, PipelineMetadata> action)
        {
            _action = action;
        }

        public void Process(TInput input, PipelineMetadata metadata)
        {
            var inp = input;
            var md = metadata;
            _action(inp, md);
        }
    }
}