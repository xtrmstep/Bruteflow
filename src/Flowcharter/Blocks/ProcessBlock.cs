using System;

namespace Flowcharter.Blocks
{
    public class ProcessBlock<TInput, TOutput> : IReceiverBlock<TInput>, IProducerBlock<TOutput>
    {
        private IReceiverBlock<TOutput> _following;
        private Func<TInput, PipelineMetadata, TOutput> _process;

        protected internal ProcessBlock() : this(null)
        {
        }

        protected internal ProcessBlock(Func<TInput, PipelineMetadata, TOutput> process)
        {
            _process = process;
        }

        void IProducerBlock<TOutput>.Link(IReceiverBlock<TOutput> receiverBlock)
        {
            _following = receiverBlock;
        }

        public void Push(TInput input, PipelineMetadata metadata)
        {
            var output = _process(input, metadata);

            _following?.Push(output, metadata);
        }

        public void Flush()
        {
            _following?.Flush();
        }
    }
}