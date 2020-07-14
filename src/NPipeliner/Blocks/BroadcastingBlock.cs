using System.Threading.Tasks;

namespace NPipeliner.Blocks
{
    public abstract class BroadcastingBlock<TOutput>
    {
        private readonly IReceiverBlock<TOutput>[] _next;

        protected BroadcastingBlock(params IReceiverBlock<TOutput>[] next)
        {
            _next = next;
        }

        protected void Broadcast(TOutput transformed, PipelineMetadata metadata)
        {
            if (_next == null) return;

            foreach (var next in _next)
            {
                next.Process(transformed, metadata);
            }
        }
    }
}