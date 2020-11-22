using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow
{
    /// <summary>
    /// Interface of a data flow pipeline
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        ///     Launch the pipeline.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task Execute(CancellationToken cancellationToken);
    }
}