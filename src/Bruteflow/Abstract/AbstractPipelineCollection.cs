using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Abstract
{
    /// <summary>
    /// A collection of pipelines which could be used as a pipeline
    /// </summary>
    /// <remarks>
    /// Add pipelines in constructor
    /// </remarks>
    public abstract class AbstractPipelineCollection : AbstractPipeline
    {
        private readonly List<AbstractPipeline> _pipelines;

        protected AbstractPipelineCollection() : this(null)
        {            
        }
        
        protected AbstractPipelineCollection(IEnumerable<AbstractPipeline> pipelines)
        {
            _pipelines = pipelines == null 
                ? new List<AbstractPipeline>() 
                : new List<AbstractPipeline>(pipelines);
        }

        /// <inheritdoc />
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var pipelineTasks = _pipelines.Select(p => p.StartAsync(cancellationToken)).ToArray();
            return Task.WhenAll(pipelineTasks);
        }
        
        protected void Add(AbstractPipeline pipeline)
        {
            _pipelines.Add(pipeline);
        }
    }
}