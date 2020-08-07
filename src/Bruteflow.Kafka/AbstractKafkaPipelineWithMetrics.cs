using System;
using System.Threading;
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

        protected override void PushToFlow(CancellationToken cancellationToken, TConsumerValue entity, PipelineMetadata pipelineMetadata)
        {
            base.PushToFlow(cancellationToken, entity, pipelineMetadata);
            Stats.Metric().PipelineLatency(pipelineMetadata);
        }

        protected override void OnError(Exception err)
        {
            base.OnError(err);
            Stats.Metric().FatalErrorIncrement();
        }
    }
}