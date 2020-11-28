using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    public static class BlockLinkageExtensions
    {
        public static void Action<TPrecedingOutput>(this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<CancellationToken, TPrecedingOutput, PipelineMetadata, Task> action)           
        {
            var next = new ActionBlock<TPrecedingOutput>(action);
            precedingBlock.Link(next);
        }
        
        public static IProducerBlock<ImmutableArray<TPrecedingOutput>> Batch<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            int batchSize)           
        {
            var next = new BatchBlock<TPrecedingOutput>(batchSize);
            precedingBlock.Link(next);
            return next;
        }
        
        public static void Decision<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<CancellationToken, TPrecedingOutput, PipelineMetadata, Task<bool>> condition,
            IReceiverBlock<TPrecedingOutput> positive,
            IReceiverBlock<TPrecedingOutput> negative)           
        {
            var next = new DecisionBlock<TPrecedingOutput>(condition);
            var conditional = (IConditionalProducerBlock<TPrecedingOutput, TPrecedingOutput>) next;
            conditional.LinkPositive(positive);
            conditional.LinkNegative(negative);
            precedingBlock.Link(next);
        }
        
        public static void Distribute<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            params IReceiverBlock<TPrecedingOutput>[] followingBlocks)           
        {
            var next = new DistributeBlock<TPrecedingOutput>();
            var producer = (IProducerBlock<TPrecedingOutput>) next;
            foreach (var followingBlock in followingBlocks) producer.Link(followingBlock);
            precedingBlock.Link(next);
        }
        
        /// <summary>
        /// Attach processing block which has input and output
        /// </summary>
        /// <param name="precedingBlock"></param>
        /// <param name="process">Routine which processes incoming data and returns transformed result</param>
        /// <typeparam name="TPrecedingOutput"></typeparam>
        /// <typeparam name="TCurrentOutput"></typeparam>
        /// <returns></returns>
        public static IProducerBlock<TCurrentOutput> Process<TPrecedingOutput, TCurrentOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<CancellationToken, TPrecedingOutput, PipelineMetadata, Task<TCurrentOutput>> process)
        {
            var next = new ProcessBlock<TPrecedingOutput, TCurrentOutput>(process);
            precedingBlock.Link(next);
            return next;
        }
    }
}