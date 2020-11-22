using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow.Blocks
{
    public static class ActionBlockExtensions
    {
        public static void Action<TPrecedingOutput>(this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<CancellationToken, TPrecedingOutput, PipelineMetadata, Task> action)           
        {
            var next = new ActionBlock<TPrecedingOutput>(action);
            precedingBlock.Link(next);
        }
    }
}