using System;
using System.Threading.Tasks;
using Bruteflow.Blocks;
using Microsoft.Extensions.DependencyInjection;

namespace Bruteflow.Abstract
{
    public class AbstractPipe<TInput> : IPipe<TInput>
    {
        private readonly IServiceProvider _serviceProvider;

        public AbstractPipe(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        /// <inheritdoc />
        public HeadBlock<TInput> Head { get; } = new HeadBlock<TInput>();
        
        protected async Task<TOutput> Scope<TRoutine, TOutput>(Func<TRoutine, Task<TOutput>> func)
        {
            using var scope = _serviceProvider.CreateScope();
            var routine = scope.ServiceProvider.GetService<TRoutine>();
            var output = await func(routine).ConfigureAwait(false);
            return output;
        }
        
        protected async Task Scope<TRoutine>(Func<TRoutine, Task> func)
        {
            using var scope = _serviceProvider.CreateScope();
            var routine = scope.ServiceProvider.GetService<TRoutine>();
            await func(routine).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            // nothing to dispose here
        }
    }
}