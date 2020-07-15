using System;

namespace Flowcharter.Blocks
{
    public class ProcessBlock<TInput, TOutput> : IReceiverBlock<TInput>, IProducerBlock<TOutput>
    {
        private Func<TInput, PipelineMetadata, TOutput> _process;
        private IReceiverBlock<TOutput> _following;

        public ProcessBlock(): this(null, null)
        {
            
        }
        
        public ProcessBlock(Func<TInput, PipelineMetadata, TOutput> process) : this(process, null)
        {
        }

        public ProcessBlock(Func<TInput, PipelineMetadata, TOutput> process, IReceiverBlock<TOutput> receiverBlock)
        {
            _process = process;
            _following = receiverBlock;
        }

        void IProducerBlock<TOutput>.Link(IReceiverBlock<TOutput> receiverBlock)
        {
            _following = receiverBlock;
        }

        public void Post(TInput input, PipelineMetadata metadata)
        {
            var output = _process(input, metadata);

            _following?.Post(output, metadata);
        }

        protected internal void SetProcess(Func<TInput, PipelineMetadata, TOutput> process)
        {
            _process = process;
        }
    }

    public static class ProcessBlockExtensions
    {
        public static ProcessBlock<TPrecedingOutput, TCurrentOutput> Next<TPrecedingOutput, TCurrentOutput>(
            this IProducerBlock<TPrecedingOutput> precedingBlock,
            Func<TPrecedingOutput, PipelineMetadata, TCurrentOutput> process)
        {
            var next = new ProcessBlock<TPrecedingOutput, TCurrentOutput>(process);
            precedingBlock.Link(next);
            return next;
        }
        
        public static ProcessBlock<TInput, TOutput> Process<TInput, TOutput>(
            this ProcessBlock<TInput, TOutput> block,
            Func<TInput, PipelineMetadata, TOutput> process)
        {
            block.SetProcess(process);
            return block;
        }
    }
}