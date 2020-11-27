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
            IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory, IServiceProvider serviceProvider) 
            : base(serviceProvider)
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
                consumerResult = await Task.Run(() =>
                {
                    var r = Consumer.Consume(cancellationToken);
                    while (r != null && r.IsPartitionEOF)
                    {
                        r = Consumer.Consume(cancellationToken);
                    }
                    return r;
                }, cancellationToken)
                    .ConfigureAwait(false);
                
            }
            catch (OperationCanceledException)
            {
                return null;
            }

            if (consumerResult == null) return null;
            
            entity.Entity = consumerResult.Message.Value;
            entity.Metadata = new PipelineMetadata {Metadata = consumerResult.Message, InputTimestamp = DateTime.Now};
            return entity;
        }

        protected override Task OnError(Exception err)
        {
            Logger.LogError(err, err.Message);
            return Task.CompletedTask;
        }
    }
}