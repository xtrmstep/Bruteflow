﻿using System;
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
                while (ReadNextEntity(cancellationToken, out var entity))
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    var pipelineMetadata = CreateMetadata(entity);
                    PushToFlow(entity, pipelineMetadata);
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

        private static PipelineMetadata CreateMetadata(TInput entity)
        {
            return new PipelineMetadata {Metadata = entity, InputTimestamp = DateTime.Now};
        }

        protected abstract bool ReadNextEntity(CancellationToken cancellationToken, out TInput entity);

        protected virtual void PushToFlow(TInput entity, PipelineMetadata pipelineMetadata)
        {
            Head.Push(entity, pipelineMetadata);
        }
    }
}