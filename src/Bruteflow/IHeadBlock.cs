using System.Threading;
using System.Threading.Tasks;

namespace Bruteflow
{
    /// <summary>
    /// Interface of a starting block for data flow pipeline  
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IHeadBlock<in TInput> : IReceiverBlock<TInput>
    {
        
    }
}