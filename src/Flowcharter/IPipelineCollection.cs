using System.Collections.Generic;

namespace Flowcharter
{
    public interface IPipelineCollection : IPipeline
    {
        IReadOnlyList<IPipeline> Pipelines { get; }
    }
}