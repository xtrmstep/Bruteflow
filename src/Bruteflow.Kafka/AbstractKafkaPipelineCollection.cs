using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Kafka
{
    public abstract class AbstractKafkaPipelineCollection : IPipelineCollection
    {
        private readonly List<IPipeline> _pipelines = new List<IPipeline>();

        public Task Execute(CancellationToken cancellationToken)
        {
            return Task.WhenAll(_pipelines.Select(pipeline => pipeline.Execute(cancellationToken)).ToArray());
        }

        public IReadOnlyList<IPipeline> Pipelines => _pipelines;

        protected void Add(IPipeline pipeline)
        {
            _pipelines.Add(pipeline);
        }
    }
}