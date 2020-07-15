using System.Threading;

namespace Flowcharter
{
    public interface IPipeline
    {
        void Execute(CancellationToken cancellationToken);
    }
}