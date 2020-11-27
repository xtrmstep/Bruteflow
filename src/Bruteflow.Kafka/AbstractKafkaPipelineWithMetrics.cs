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
            IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory, IMetricsPublisher stats, IServiceProvider serviceProvider)
            : base(logger, consumerFactory, serviceProvider)
        {
            Stats = stats;
        }

        protected override async Task PushToFlow(CancellationToken cancellationToken, TConsumerValue entity, PipelineMetadata pipelineMetadata)
        {
            await base.PushToFlow(cancellationToken, entity, pipelineMetadata).ConfigureAwait(false);
            await Stats.Metric().PipelineLatency(pipelineMetadata).ConfigureAwait(false);
        }

        protected override async Task OnError(Exception err)
        {
            await base.OnError(err).ConfigureAwait(false);
            await Stats.Metric().FatalErrorIncrement().ConfigureAwait(false);
        }
    }
}