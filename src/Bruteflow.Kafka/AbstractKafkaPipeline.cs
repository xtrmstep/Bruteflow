using System;
using System.Threading;
using Bruteflow.Abstract;
using Bruteflow.Kafka.Consumers;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka
{
    public abstract class AbstractKafkaPipeline<TConsumerKey, TConsumerValue> : AbstractPipeline<TConsumerValue>
    {
        protected readonly IKafkaConsumer<TConsumerKey, TConsumerValue> Consumer;
        protected readonly ILogger Logger;

        protected AbstractKafkaPipeline(ILogger<AbstractKafkaPipeline<TConsumerKey, TConsumerValue>> logger,
            IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory)
        {
            Logger = logger;
            Consumer = consumerFactory.CreateConsumer();
        }

        protected override bool ReadNextEntity(CancellationToken cancellationToken, out TConsumerValue entity)
        {
            entity = default;
            ConsumeResult<TConsumerKey, TConsumerValue> consumerResult;
            do
            {
                if (cancellationToken.IsCancellationRequested) return false;

                consumerResult = Consumer.Consume(cancellationToken);
            } while (!consumerResult.IsPartitionEOF);

            entity = consumerResult.Message.Value;
            return true;
        }

        protected override void PushToFlow(TConsumerValue entity, PipelineMetadata pipelineMetadata)
        {
            Head.Push(entity, pipelineMetadata);
        }

        protected override void OnError(Exception err)
        {
            Logger.LogError(err, err.Message);
        }
    }
}