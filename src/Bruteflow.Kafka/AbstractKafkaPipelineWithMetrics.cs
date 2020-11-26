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

        protected override Task PushToFlow(CancellationToken cancellationToken, TConsumerValue entity, PipelineMetadata pipelineMetadata)
        {
            return base.PushToFlow(cancellationToken, entity, pipelineMetadata)
                .ContinueWith(antecedent => Stats.Metric().PipelineLatency(pipelineMetadata), cancellationToken);
        }

        protected override Task OnError(Exception err)
        {
            return base.OnError(err)
                .ContinueWith(antecedent => Stats.Metric().FatalErrorIncrement());
        }
    }
}