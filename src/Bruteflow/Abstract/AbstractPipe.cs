using Bruteflow.Blocks;

namespace Bruteflow.Abstract
{
    public class AbstractPipe<TInput> : IPipe<TInput>
    {
        /// <inheritdoc />
        public HeadBlock<TInput> Head { get; } = new HeadBlock<TInput>();
    }
}