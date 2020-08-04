using System.Threading;

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
        void Execute(CancellationToken cancellationToken);
    }
}