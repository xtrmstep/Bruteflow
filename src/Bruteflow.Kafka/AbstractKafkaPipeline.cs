using System;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Abstract;
using Bruteflow.Kafka.Consumers;
using Bruteflow.Kafka.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka
{
    public abstract class AbstractKafkaPipeline<TConsumerKey, TConsumerValue, TPipe> : AbstractPipeline<TConsumerValue, TPipe>
        where TPipe : AbstractPipe<TConsumerValue>
    {
        protected readonly IKafkaConsumer<TConsumerKey, TConsumerValue> Consumer;
        protected readonly ILogger<AbstractKafkaPipeline<TConsumerKey, TConsumerValue, TPipe>> Logger;

        protected AbstractKafkaPipeline(ILogger<AbstractKafkaPipeline<TConsumerKey, TConsumerValue, TPipe>> logger,
            IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory,
            IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            Logger = logger;
            Consumer = consumerFactory.CreateConsumer();
        }

        protected override async Task<DataItem<TConsumerValue>> FetchNextDataAsync(CancellationToken cancellationToken)
        {
            var entity = new DataItem<TConsumerValue>();
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
            
            entity.Data = consumerResult.Message.Value;
            entity.Metadata = new PipelineMetadata {Metadata = consumerResult.Message, Timestamp = DateTime.Now};
            return entity;
        }

        protected override Task OnFatalErrorAsync(Exception err)
        {
            Logger.LogError(err, err.Message);
            return Task.CompletedTask;
        }
    }
}