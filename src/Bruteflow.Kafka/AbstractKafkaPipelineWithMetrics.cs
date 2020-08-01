using System;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Stats;
using Confluent.Kafka;
using JustEat.StatsD;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka
{
    public abstract class AbstractKafkaPipelineWithMetrics<TConsumerKey, TConsumerValue> : AbstractKafkaPipeline<TConsumerKey, TConsumerValue>
    {
        protected readonly IStatsDPublisher Stats;

        protected AbstractKafkaPipelineWithMetrics(ILogger<AbstractKafkaPipelineWithMetrics<TConsumerKey, TConsumerValue>> logger,
            IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory, IStatsDPublisher stats)
            : base(logger, consumerFactory)
        {
            Stats = stats;
        }

        protected override void PushToFlow(Message<TConsumerKey, TConsumerValue> message, PipelineMetadata pipelineMetadata)
        {
            try
            {
                base.PushToFlow(message, pipelineMetadata);
                Stats.Metric().PipelineLatency(pipelineMetadata);
            }
            catch (Exception err)
            {
                Stats.Metric().FatalErrorIncrement();
                Logger.LogError(err, err.Message);
                throw;
            }
        }
    }
}