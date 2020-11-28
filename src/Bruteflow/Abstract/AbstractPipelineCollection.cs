using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Abstract
{
    /// <summary>
    /// A base class for building a collection of data flow pipelines
    /// </summary>
    /// <remarks>
    /// You need to define a data glow pipeline in the constructor of your class, attaching blocks to the Head block
    /// </remarks>
    public abstract class AbstractPipelineCollection : IPipelineCollection
    {
        protected AbstractPipelineCollection(IReadOnlyList<IPipeline> pipelines)
        {
            Pipelines = pipelines;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var pipelineTasks = Pipelines.Select(p => p.StartAsync(cancellationToken)).ToArray();
            return Task.WhenAll(pipelineTasks);
        }

        /// <inheritdoc />
        public IReadOnlyList<IPipeline> Pipelines { get; }
    }
}