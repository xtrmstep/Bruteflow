using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow
{
    /// <summary>
    /// Interface of a starting block for data flow pipeline  
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IHeadBlock<in TInput> : IReceiverBlock<TInput>
    {
        /// <summary>
        /// The block may have a method (an internal generator) which pushes event to the pipeline. Call Start() to launch the internal generator
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task Start(CancellationToken cancellationToken);
    }
}