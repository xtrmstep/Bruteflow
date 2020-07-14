using System.Threading;

namespace NPipeliner
{
    public interface IPipeline
    {
        void Execute(CancellationToken cancellationToken);
    }
}