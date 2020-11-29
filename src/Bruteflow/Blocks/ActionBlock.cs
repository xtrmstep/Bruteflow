using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     Ending block of a chained dataflow pipeline
    /// </summary>
    /// <typeparam name="TInput">Data type which the block receives</typeparam>
    public sealed class ActionBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Func<CancellationToken, TInput, PipelineMetadata, Task> _action;

        internal ActionBlock(Func<CancellationToken, TInput, PipelineMetadata, Task> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action), "Cannot be null");
        }

        public async Task PushAsync(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata)
        {
            await _action(cancellationToken, input, metadata).ConfigureAwait(false);
        }

        public Task FlushAsync(CancellationToken cancellationToken)
        {
            // do nothing
            return Task.CompletedTask;
        }
    }
}