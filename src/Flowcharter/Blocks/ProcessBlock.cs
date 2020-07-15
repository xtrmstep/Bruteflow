using System;

namespace Flowcharter.Blocks
{
    public class ProcessBlock<TInput, TOutput> : IReceiverBlock<TInput>
    {
        private readonly Func<TInput, PipelineMetadata, TOutput> _process;
        private readonly IReceiverBlock<TOutput> _next;

        public ProcessBlock(Func<TInput, PipelineMetadata, TOutput> process, IReceiverBlock<TOutput> next)
        {
            _process = process;
            _next = next;
        }

        public void Process(TInput input, PipelineMetadata metadata)
        {
            var output = _process(input, metadata);

            _next?.Process(output, metadata);
        }
    }
}