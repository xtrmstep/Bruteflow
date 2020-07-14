using System.Threading.Tasks;

namespace NPipeliner.Blocks
{
    public class BroadcastingBlock<TOutput>
    {
        private readonly IReceiverBlock<TOutput>[] _next;

        protected BroadcastingBlock(params IReceiverBlock<TOutput>[] next)
        {
            _next = next;
        }

        protected void Broadcast(TOutput transformed, PipelineMetadata metadata)
        {
            if (_next == null) return;

            Parallel.ForEach(_next, next =>
            {
                var inp = transformed;
                var md = metadata;
                next.Process(inp, md);
            });
        }
    }
}