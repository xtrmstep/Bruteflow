using System;
using System.Threading;
using System.Threading.Tasks;
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

        protected override async Task<EntityItem<TConsumerValue>> ReadNextEntity(CancellationToken cancellationToken)
        {
            var entity = new EntityItem<TConsumerValue>();
            ConsumeResult<TConsumerKey, TConsumerValue> consumerResult;
            try
            {
                consumerResult = await Consumer.Consume(cancellationToken).ConfigureAwait(false);
                while (consumerResult != null && consumerResult.IsPartitionEOF)
                {
                    consumerResult = await Consumer.Consume(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException err)
            {
                return null;
            }

            if (consumerResult == null) return null;
            
            entity.Entity = consumerResult.Message.Value;
            entity.Metadata = new PipelineMetadata {Metadata = consumerResult.Message, InputTimestamp = DateTime.Now};
            return entity;
        }

        protected override Task PushToFlow(CancellationToken cancellationToken, TConsumerValue entity, PipelineMetadata pipelineMetadata)
        {
            return Head.Push(cancellationToken, entity, pipelineMetadata);
        }

        protected override Task OnError(Exception err)
        {
            Logger.LogError(err, err.Message);
            return Task.CompletedTask;
        }
    }
}