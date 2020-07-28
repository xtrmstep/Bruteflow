using System;

namespace Bruteflow.Blocks
{
    public class ActionBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Action<TInput, PipelineMetadata> _action;

        protected internal ActionBlock(Action<TInput, PipelineMetadata> action)
        {
            _action = action;
        }

        public void Push(TInput input, PipelineMetadata metadata)
        {
            var inp = input;
            var md = metadata;
            _action(inp, md);
        }

        public void Flush()
        {
            // do nothing
        }
    }
}