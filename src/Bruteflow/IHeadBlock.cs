using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow
{
    /// <summary>
    /// Interface of a starting block for data flow pipeline  
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IHeadBlock<in TInput>
    {
        /// <summary>
        /// The block may have a method (an internal generator) which pushes event to the pipeline. Call Start() to launch the internal generator
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Push a single data entity to the pipeline
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="input">Data entity</param>
        /// <param name="metadata">Metadata accompanying data entry</param>
        Task Push(CancellationToken cancellationToken, TInput input, PipelineMetadata metadata);
        /// <summary>
        /// Initiate the purging of internal states of the pipeline. Incomplete batches will be propagated to further blocks in chain
        /// </summary>
        Task Flush(CancellationToken cancellationToken);
    }
}