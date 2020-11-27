using System;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Blocks;
using Microsoft.Extensions.DependencyInjection;

namespace Bruteflow.Abstract
{
    /// <summary>
    /// A base class for building a complex data flow pipeline
    /// </summary>
    /// <remarks>
    /// You need to define a data glow pipeline in the constructor of your class, attaching blocks to the Head block
    /// </remarks>
    /// <typeparam name="TInput"></typeparam>
    public abstract class AbstractPipeline<TInput> : IPipeline
    {
        private readonly IServiceProvider _serviceProvider;

        protected AbstractPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task Execute(CancellationToken cancellationToken)
        {
            try
            {
                EntityItem<TInput> nextEntity;
                while ((nextEntity = await ReadNextEntity(cancellationToken).ConfigureAwait(false)) != null)
                {
                    if (cancellationToken.IsCancellationRequested) break;                    
                    PushToFlow(cancellationToken, nextEntity.Entity, nextEntity.Metadata);
                }                
            }
            catch (Exception err)
            {
                await OnError(err).ConfigureAwait(false);
                throw;
            }
        }
        
        /// <summary>
        /// Overload this method to define special behaviour after a fatal error, when execution of the pipeline stopped   
        /// </summary>
        /// <param name="err"></param>
        protected virtual Task OnError(Exception err)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// Implement this method to read and return data entities
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task<EntityItem<TInput>> ReadNextEntity(CancellationToken cancellationToken);

        /// <summary>
        /// Pushes a data entity to the internal block chain
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entity"></param>
        /// <param name="pipelineMetadata"></param>
        protected virtual void PushToFlow(CancellationToken cancellationToken, TInput entity, PipelineMetadata pipelineMetadata)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var pipe = CreatePipe(scope.ServiceProvider);
                await pipe.Head.Push(cancellationToken, entity, pipelineMetadata).ConfigureAwait(false);
            }, cancellationToken);
        }

        protected abstract IPipe<TInput> CreatePipe(IServiceProvider scopeServiceProvider);
    }
}