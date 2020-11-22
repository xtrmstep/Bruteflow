using System;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Blocks;

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
        /// <summary>
        /// The head of the main pipeline 
        /// </summary>
        protected readonly HeadBlock<TInput> Head = new HeadBlock<TInput>();

        public async Task Execute(CancellationToken cancellationToken)
        {
            try
            {
                while (await ReadNextEntity(cancellationToken, out var entity, out var metadata))
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    await PushToFlow(cancellationToken, entity, metadata);
                }
            }
            catch (Exception err)
            {
                await OnError(err);
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
        /// <param name="entity">Data entity</param>
        /// <param name="pipelineMetadata"></param>
        /// <returns></returns>
        protected abstract Task<bool> ReadNextEntity(CancellationToken cancellationToken, out TInput entity, out PipelineMetadata pipelineMetadata);

        /// <summary>
        /// Pushes a data entity to the internal block chain
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entity"></param>
        /// <param name="pipelineMetadata"></param>
        protected virtual Task PushToFlow(CancellationToken cancellationToken, TInput entity, PipelineMetadata pipelineMetadata)
        {
            return Head.Push(cancellationToken, entity, pipelineMetadata);
        }
    }
}