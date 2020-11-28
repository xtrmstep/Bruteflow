﻿using System;
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
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                DataItem<TInput> nextData;
                while ((nextData = await FetchNextDataAsync(cancellationToken).ConfigureAwait(false)) != null)
                {
                    if (cancellationToken.IsCancellationRequested) break;                    
                    PushToPipe(cancellationToken, nextData.Entity, nextData.Metadata);
                }                
            }
            catch (Exception err)
            {
                await OnFatalErrorAsync(err).ConfigureAwait(false);
                throw;
            }
        }
        
        /// <summary>
        /// Overload this method to define special behaviour after a fatal error, when execution of the pipeline stopped   
        /// </summary>
        /// <param name="err"></param>
        protected virtual Task OnFatalErrorAsync(Exception err)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// Implement this method to fetch next data item to push it to the pipeline
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task<DataItem<TInput>> FetchNextDataAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Pushes a data entity to the internal block chain (starts a new task)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entity"></param>
        /// <param name="pipelineMetadata"></param>
        protected virtual void PushToPipe(CancellationToken cancellationToken, TInput entity, PipelineMetadata pipelineMetadata)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var pipe = CreatePipe(scope.ServiceProvider);
                await pipe.Head.PushAsync(cancellationToken, entity, pipelineMetadata).ConfigureAwait(false);
            }, cancellationToken);
        }

        protected abstract IPipe<TInput> CreatePipe(IServiceProvider scopeServiceProvider);
    }
}