using System.Threading.Tasks;

namespace Flowcharter.Blocks
{
    public class DistributeBlock<TEntity> : IReceiverBlock<TEntity>
    {
        private readonly IReceiverBlock<TEntity>[] _targets;

        public DistributeBlock(params IReceiverBlock<TEntity>[] targets)
        {
            _targets = targets;
        }

        public void Process(TEntity input, PipelineMetadata metadata)
        {
            if (_targets == null) return;

            foreach (var target in _targets)
            {
                target.Process(input, metadata);
            }
        }
    }
}