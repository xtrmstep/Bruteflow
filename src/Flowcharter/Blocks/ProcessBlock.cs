using System;

namespace Flowcharter.Blocks
{
    public class ProcessBlock<TInput, TOutput> : IReceiverBlock<TInput>, IProducerBlock<TOutput>
    {
        private readonly Func<TInput, PipelineMetadata, TOutput> _process;
        private IReceiverBlock<TOutput> _following;

        public ProcessBlock(Func<TInput, PipelineMetadata, TOutput> process) : this(process, null)
        {
        }

        public ProcessBlock(Func<TInput, PipelineMetadata, TOutput> process, IReceiverBlock<TOutput> receiverBlock)
        {
            _process = process;
            _following = receiverBlock;
        }

        public void Link(IReceiverBlock<TOutput> receiverBlock)
        {
            _following = receiverBlock;
        }

        public void Process(TInput input, PipelineMetadata metadata)
        {
            var output = _process(input, metadata);

            _following?.Process(output, metadata);
        }
    }

    public static class ProcessBlockExtensions
    {
        public static ProcessBlock<TPrecedingOutput, TCurrentOutput> Process<TPrecedingOutput, TCurrentOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<TPrecedingOutput, PipelineMetadata, TCurrentOutput> process)
        {
            var next = new ProcessBlock<TPrecedingOutput, TCurrentOutput>(process);
            precedingBlock.Link(next);
            return next;
        }
    }
}