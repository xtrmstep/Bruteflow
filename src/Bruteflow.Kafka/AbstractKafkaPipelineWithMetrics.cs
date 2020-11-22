using System;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Stats;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka
{
    public abstract class AbstractKafkaPipelineWithMetrics<TConsumerKey, TConsumerValue> : AbstractKafkaPipeline<TConsumerKey, TConsumerValue>
    {
        protected readonly IMetricsPublisher Stats;

        protected AbstractKafkaPipelineWithMetrics(ILogger<AbstractKafkaPipelineWithMetrics<TConsumerKey, TConsumerValue>> logger,
            IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory, IMetricsPublisher stats)
            : base(logger, consumerFactory)
        {
            Stats = stats;
        }

        protected override async Task PushToFlow(CancellationToken cancellationToken, TConsumerValue entity, PipelineMetadata pipelineMetadata)
        {
            await base.PushToFlow(cancellationToken, entity, pipelineMetadata);
            await Stats.Metric().PipelineLatency(pipelineMetadata);
        }

        protected override async Task OnError(Exception err)
        {
            await base.OnError(err);
            await Stats.Metric().FatalErrorIncrement();
        }
    }
}