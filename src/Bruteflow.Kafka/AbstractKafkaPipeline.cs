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
    public abstract class AbstractKafkaPipeline<TConsumerKey, TConsumerValue> : IPipeline
    {
        protected readonly ILogger Logger;
        protected readonly IStatsDPublisher Stats;
        protected readonly IKafkaConsumer<TConsumerKey, TConsumerValue> Consumer;
        protected readonly HeadBlock<TConsumerValue> Head = new HeadBlock<TConsumerValue>();

        protected AbstractKafkaPipeline(IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory,
            ILogger logger, IStatsDPublisher stats)
        {
            Logger = logger;
            Stats = stats;
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
                Stats.Metric().FatalErrorIncrement();
                Logger.LogError(err, err.Message);
                throw;
            }
        }
    }
}