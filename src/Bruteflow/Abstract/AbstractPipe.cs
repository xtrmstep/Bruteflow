using System;
using System.Threading.Tasks;
using Bruteflow.Blocks;
using Microsoft.Extensions.DependencyInjection;

namespace Bruteflow.Abstract
{
    /// <summary>
    /// The definition of a chained blocks and its input
    /// </summary>
    /// <typeparam name="TInput">Data type which will be pushed to the input</typeparam>
    public abstract class AbstractPipe<TInput>
    {
        private readonly IServiceProvider _serviceProvider;

        protected AbstractPipe(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        /// <summary>
        /// The first block in the pipe which is its input
        /// </summary>
        public HeadBlock<TInput> Head { get; } = new HeadBlock<TInput>();
        
        /// <summary>
        /// Execute routine method in a scoped context
        /// </summary>
        protected async Task<TOutput> Scope<TRoutine, TOutput>(Func<TRoutine, Task<TOutput>> func)
        {
            using var scope = _serviceProvider.CreateScope();
            var routine = scope.ServiceProvider.GetService<TRoutine>();
            var output = await func(routine).ConfigureAwait(false);
            return output;
        }
        
        /// <summary>
        /// Execute routine method in a scoped context
        /// </summary>
        protected async Task Scope<TRoutine>(Func<TRoutine, Task> func)
        {
            using var scope = _serviceProvider.CreateScope();
            var routine = scope.ServiceProvider.GetService<TRoutine>();
            await func(routine).ConfigureAwait(false);
        }
    }
}