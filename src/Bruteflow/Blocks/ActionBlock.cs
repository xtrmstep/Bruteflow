using System;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     Ending block of the chained flow pipeline
    /// </summary>
    /// <typeparam name="TInput">Data type which the block receives</typeparam>
    public sealed class ActionBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Action<TInput, PipelineMetadata> _action;

        internal ActionBlock(Action<TInput, PipelineMetadata> action)
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