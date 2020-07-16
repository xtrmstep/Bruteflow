using System;
using System.Threading;

namespace Flowcharter.Blocks
{
    public class HeadBlock<TOutput> : IHeadBlock, IReceiverBlock<TOutput>, IProducerBlock<TOutput>
    {
        private IReceiverBlock<TOutput> _following;
        private readonly Action<Action<TOutput, PipelineMetadata>> _process;

        public HeadBlock() : this(null)
        {
        }
        
        public HeadBlock(Action<Action<TOutput, PipelineMetadata>> process)
        {
            _process = process;
        }

        void IProducerBlock<TOutput>.Link(IReceiverBlock<TOutput> receiverBlock)
        {
            _following = receiverBlock;
        }

        public void Start()
        {
            _process(_following.Push);
        }

        void IReceiverBlock<TOutput>.Push(TOutput input, PipelineMetadata metadata)
        {
            _following.Push(input, metadata);
        }
    }
}