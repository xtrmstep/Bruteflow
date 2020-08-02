using System.Threading;

namespace Bruteflow
{
    public interface IPipeline
    {
        /// <summary>
        ///     Launch the pipeline.
        /// </summary>
        /// <param name="cancellationToken"></param>
        void Execute(CancellationToken cancellationToken);
    }
}