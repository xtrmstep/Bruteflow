using System.Collections.Generic;

namespace Bruteflow
{
    public interface IPipelineCollection : IPipeline
    {
        IReadOnlyList<IPipeline> Pipelines { get; }
    }
}