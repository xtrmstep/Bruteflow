using System.Threading;

namespace Bruteflow
{
    public interface IPipeline
    {
        void Execute(CancellationToken cancellationToken);
    }
}