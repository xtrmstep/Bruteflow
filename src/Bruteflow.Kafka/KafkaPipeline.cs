using System;
using System.Threading;
using Bruteflow;
using Bruteflow.Blocks;
using JustEat.StatsD;
using Microsoft.Extensions.Logging;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Stats;

namespace Bruteflow.Kafka
{
    public abstract class KafkaPipeline<TConsumerKey, TConsumerValue> : IPipeline
    {
        private readonly ILogger _logger;
        private readonly IStatsDPublisher _stats;
        protected readonly IKafkaConsumer<TConsumerKey, TConsumerValue> Consumer;
        protected readonly HeadBlock<TConsumerValue> Head = new HeadBlock<TConsumerValue>();

        protected KafkaPipeline(IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory,
            ILogger logger, IStatsDPublisher stats)
        {
            _logger = logger;
            _stats = stats;
            Consumer = consumerFactory.CreateConsumer();
        }

        public void Execute(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    var consumerResult = Consumer.Consume(cancellationToken);
                    var message = consumerResult.Message;
                    var pipelineMetadata = new PipelineMetadata {Metadata = message, InputTimestamp = DateTime.Now};

                    if (consumerResult.IsPartitionEOF) continue;

                    Head.Push(message.Value, pipelineMetadata);
                }
            }
            catch (Exception err)
            {
                _stats.Measure().CountCrashes();
                _logger.LogError(err, err.Message);
                throw;
            }
        }

        protected bool IsNoNullValue(object value)
        {
            if (value != null) return true;

            _stats.Measure().CountWarnings();

            return false;
        }
    }
}