using System;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Abstract;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Settings;
using Bruteflow.Kafka.Stats;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka
{
    public abstract class AbstractKafkaPipelineWithMetrics<TConsumerKey, TConsumerValue, TPipe> : AbstractKafkaPipeline<TConsumerKey, TConsumerValue, TPipe>
        where TPipe : AbstractPipe<TConsumerValue>
    {
        private readonly IMetricsPublisher _stats;

        protected AbstractKafkaPipelineWithMetrics(ILogger<AbstractKafkaPipelineWithMetrics<TConsumerKey, TConsumerValue, TPipe>> logger,
            IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory,
            IMetricsPublisher stats,
            IServiceProvider serviceProvider)
            : base(logger, consumerFactory, serviceProvider)
        {
            _stats = stats;
        }

        protected override async Task PushToPipeAsync(CancellationToken cancellationToken, TConsumerValue entity, PipelineMetadata pipelineMetadata, AbstractPipe<TConsumerValue> pipe)
        {
            await pipe.Head.PushAsync(cancellationToken, entity, pipelineMetadata).ConfigureAwait(false);
            await _stats.Metric().PipelineLatency(pipelineMetadata).ConfigureAwait(false);
        }

        protected override async Task OnFatalErrorAsync(Exception err)
        {
            await base.OnFatalErrorAsync(err).ConfigureAwait(false);
            await _stats.Metric().FatalErrorIncrement().ConfigureAwait(false);
        }
    }
}