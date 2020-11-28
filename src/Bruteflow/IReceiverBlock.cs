using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow
{
    /// <summary>
    /// Interface of a block which can receive data
    /// </summary>
    /// <typeparam name="TInput">Data type which block receives</typeparam>
    public interface IReceiverBlock<in TInput>
    {
        // Push an entity to this block 
        Task PushAsync(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata);

        /// <summary>
        ///     Push internal state to following blocks and flush the state
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task FlushAsync(CancellationToken cancellationToken);
    }
}