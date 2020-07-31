using System.Collections.Generic;
using System.Threading;
using Bruteflow;

namespace Bruteflow.Kafka
{
    public abstract class KafkaPipelineCollection : IPipelineCollection
    {
        private readonly List<IPipeline> _pipelines = new List<IPipeline>();

        public void Execute(CancellationToken cancellationToken)
        {
            foreach (var pipeline in _pipelines) ThreadPool.QueueUserWorkItem(data => pipeline.Execute(cancellationToken));
        }

        public IReadOnlyList<IPipeline> Pipelines => _pipelines;

        protected void Add(IPipeline pipeline)
        {
            _pipelines.Add(pipeline);
        }
    }
}