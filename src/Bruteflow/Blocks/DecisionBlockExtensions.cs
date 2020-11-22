using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    public static class DecisionBlockExtensions
    {
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
    }
}