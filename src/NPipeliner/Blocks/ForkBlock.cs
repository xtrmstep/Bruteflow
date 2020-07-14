using System.Threading.Tasks;

namespace NPipeliner.Blocks
{
    public class ForkBlock<TInput> : IReceiverBlock<TInput>
    {
        private readonly IReceiverBlock<TInput>[] _branches;

        public ForkBlock(params IReceiverBlock<TInput>[] branches)
        {
            _branches = branches;
        }

        public void Process(TInput input, PipelineMetadata metadata)
        {
            Parallel.ForEach(_branches, branch =>
            {
                var inp = input;
                var md = metadata;
                branch.Process(inp, md);
            });
        }
    }
}