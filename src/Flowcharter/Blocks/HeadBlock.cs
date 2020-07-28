using System;
using System.Threading;

namespace Flowcharter.Blocks
{
    public class HeadBlock<TInput> : IHeadBlock<TInput>, IReceiverBlock<TInput>, IProducerBlock<TInput>
    {
        private IReceiverBlock<TInput> _following;
        private readonly Action<Action<TInput, PipelineMetadata>> _process;

        public HeadBlock() : this(null)
        {
        }
        
        public HeadBlock(Action<Action<TInput, PipelineMetadata>> process)
        {
            _process = process;
        }

        void IProducerBlock<TInput>.Link(IReceiverBlock<TInput> receiverBlock)
        {
            _following = receiverBlock;
        }

        public void Start()
        {
            _process(_following.Push);
        }

        public void Push(TInput input, PipelineMetadata metadata)
        {
            _following.Push(input, metadata);
        }
    }
}