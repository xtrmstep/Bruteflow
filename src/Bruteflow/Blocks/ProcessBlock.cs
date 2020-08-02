using System;

namespace Bruteflow.Blocks
{
    /// <summary>
    /// The block which applies a transformation or other logic to incoming entity, before pushing it further
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public sealed class ProcessBlock<TInput, TOutput> : IReceiverBlock<TInput>, IProducerBlock<TOutput>
    {
        private readonly Func<TInput, PipelineMetadata, TOutput> _process;
        private IReceiverBlock<TOutput> _following;

        internal ProcessBlock() : this(null)
        {
        }

        internal ProcessBlock(Func<TInput, PipelineMetadata, TOutput> process)
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