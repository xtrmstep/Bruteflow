using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bruteflow.Abstract
{
    public abstract class AbstractPipeline
    {
        /// <summary>
        ///     Launch the pipeline.
        /// </summary>
        public abstract Task StartAsync(CancellationToken cancellationToken);
    }
    
    /// <summary>
    /// The definition of the logic to fetch data for the pipeline
    /// </summary>
    /// <remarks>
    /// The pipeline is a class to connect data fetching logic and the pipe (chained blocks).
    /// When a new data item arrives, a new scope is created from which a new pipe is resolved.  
    /// </remarks>
    /// <typeparam name="TInput">The input data type for the pipe</typeparam>
    /// <typeparam name="TPipe">The pipe definition class</typeparam>
    public abstract class AbstractPipeline<TInput, TPipe> : AbstractPipeline
        where TPipe: AbstractPipe<TInput>
    {
        private readonly IServiceProvider _serviceProvider;

        protected AbstractPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                DataItem<TInput> nextData;
                while ((nextData = await FetchNextDataAsync(cancellationToken).ConfigureAwait(false)) != null)
                {
                    if (cancellationToken.IsCancellationRequested) break;                    
                    PushToPipe(cancellationToken, nextData.Data, nextData.Metadata);
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
        /// Implement this method to fetch next data item to push it to the pipe
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
        private void PushToPipe(CancellationToken cancellationToken, TInput entity, PipelineMetadata pipelineMetadata)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var pipe = scope.ServiceProvider.GetService<TPipe>();
                await PushToPipeAsync(cancellationToken, entity, pipelineMetadata, pipe).ConfigureAwait(false);
            }, cancellationToken);
        }

        protected virtual async Task PushToPipeAsync(CancellationToken cancellationToken, TInput entity, PipelineMetadata pipelineMetadata, AbstractPipe<TInput> pipe)
        {
            await pipe.Head.PushAsync(cancellationToken, entity, pipelineMetadata).ConfigureAwait(false);
        }
    }
}