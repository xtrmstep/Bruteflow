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

        protected override void PushToPipe(CancellationToken cancellationToken, TConsumerValue entity, PipelineMetadata pipelineMetadata)
        {
            base.PushToPipe(cancellationToken, entity, pipelineMetadata);
            Stats.Metric().PipelineLatency(pipelineMetadata).GetAwaiter().GetResult();
        }

        protected override async Task OnFatalErrorAsync(Exception err)
        {
            await base.OnFatalErrorAsync(err).ConfigureAwait(false);
            await Stats.Metric().FatalErrorIncrement().ConfigureAwait(false);
        }
    }
}