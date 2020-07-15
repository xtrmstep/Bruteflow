using System;

namespace Flowcharter.Blocks
{
    public class ActionBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly Action<TInput, PipelineMetadata> _action;

        public ActionBlock(Action<TInput, PipelineMetadata> action)
        {
            _action = action;
        }

        public void Post(TInput input, PipelineMetadata metadata)
        {
            var inp = input;
            var md = metadata;
            _action(inp, md);
        }
    }

    public static class ActionBlockExtensions
    {
        public static void Action<TPrecedingOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Action<TPrecedingOutput, PipelineMetadata> action)
        {
            var next = new ActionBlock<TPrecedingOutput>(action);
            precedingBlock.Link(next);
        }
    }
}