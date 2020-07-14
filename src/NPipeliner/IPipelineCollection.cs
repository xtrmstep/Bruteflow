using System.Collections.Generic;

namespace NPipeliner
{
    public interface IPipelineCollection : IPipeline
    {
        IReadOnlyList<IPipeline> Pipelines { get; }
    }
}