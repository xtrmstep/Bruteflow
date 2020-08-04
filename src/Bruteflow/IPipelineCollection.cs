using System.Collections.Generic;

namespace Bruteflow
{
    /// <summary>
    /// Interface of a collection of pipelines
    /// </summary>
    public interface IPipelineCollection : IPipeline
    {
        IReadOnlyList<IPipeline> Pipelines { get; }
    }
}