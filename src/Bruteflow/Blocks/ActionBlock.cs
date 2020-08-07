using System;
using System.Threading;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     Ending block of a chained dataflow pipeline
    /// </summary>
    /// <typeparam name="TInput">Data type which the block receives</typeparam>
    public sealed class ActionBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Action<CancellationToken, TInput, PipelineMetadata> _action;

        internal ActionBlock(Action<CancellationToken, TInput, PipelineMetadata> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action), "Cannot be null");
        }

        public void Push(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            var inp = input;
            var md = metadata;
            _action(cancellationToken, inp, md);
        }

        public void Flush(CancellationToken cancellationToken)
        {
            // do nothing
        }
    }
}