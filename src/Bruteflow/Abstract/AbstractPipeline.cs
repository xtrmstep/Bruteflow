using System;
using System.Threading;
using Bruteflow.Blocks;

namespace Bruteflow.Abstract
{
    public abstract class AbstractPipeline<TInput> : IPipeline
    {
        protected readonly HeadBlock<TInput> Head = new HeadBlock<TInput>();

        public void Execute(CancellationToken cancellationToken)
        {
            try
            {
                while (ReadNextEntity(cancellationToken, out var entity, out var metadata))
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    PushToFlow(entity, metadata);
                }
            }
            catch (Exception err)
            {
                OnError(err);
                throw;
            }
        }

        protected virtual void OnError(Exception err)
        {
            // do nothing
        }

        protected abstract bool ReadNextEntity(CancellationToken cancellationToken, out TInput entity, out PipelineMetadata pipelineMetadata);

        protected virtual void PushToFlow(TInput entity, PipelineMetadata pipelineMetadata)
        {
            Head.Push(entity, pipelineMetadata);
        }
    }
}