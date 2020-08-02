using System;

namespace Bruteflow.Blocks
{
    /// <summary>
    ///     The starting block for any Bruteflow pipeline or branch
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public sealed class HeadBlock<TInput> : IHeadBlock<TInput>, IReceiverBlock<TInput>, IProducerBlock<TInput>
    {
        private readonly Action<Action<TInput, PipelineMetadata>> _process;
        private IReceiverBlock<TInput> _following;

        public HeadBlock() : this(null)
        {
        }

        public HeadBlock(Action<Action<TInput, PipelineMetadata>> process)
        {
            _process = process;
        }

        public void Start()
        {
            _process(_following.Push);
        }

        public void Push(TInput input, PipelineMetadata metadata)
        {
            _following?.Push(input, metadata);
        }

        public void Flush()
        {
            _following?.Flush();
        }

        void IProducerBlock<TInput>.Link(IReceiverBlock<TInput> receiverBlock)
        {
            _following = receiverBlock;
        }
    }
}