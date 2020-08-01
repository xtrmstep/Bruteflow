using System;
using System.Threading;
using Bruteflow.Blocks;
using Bruteflow.Kafka.Consumers;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bruteflow.Kafka
{
    public abstract class AbstractKafkaPipeline<TConsumerKey, TConsumerValue> : IPipeline
    {
        protected readonly IKafkaConsumer<TConsumerKey, TConsumerValue> Consumer;
        protected readonly HeadBlock<TConsumerValue> Head = new HeadBlock<TConsumerValue>();
        protected readonly ILogger Logger;

        protected AbstractKafkaPipeline(ILogger<AbstractKafkaPipeline<TConsumerKey, TConsumerValue>> logger,
            IConsumerFactory<TConsumerKey, TConsumerValue> consumerFactory)
        {
            Logger = logger;
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

                    PushToFlow(message, pipelineMetadata);
                }
            }
            catch (Exception err)
            {
                Logger.LogError(err, err.Message);
                throw;
            }
        }

        protected virtual void PushToFlow(Message<TConsumerKey, TConsumerValue> message, PipelineMetadata pipelineMetadata)
        {
            Head.Push(message.Value, pipelineMetadata);
        }
    }
}