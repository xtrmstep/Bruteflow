using System.Threading;
using Bruteflow.Blocks;
using FluentAssertions;
using Xunit;

namespace Bruteflow.Tests
{
    public class CreationOfBlocksTests
    {        
        [Fact]
        public void Decision_pipeline_has_only_positive_branch()
        {
            var result = string.Empty;

            var head = new HeadBlock<string>();
            var positive = new HeadBlock<string>();
            
            head.Process((ct, str, md) => str + "A")
                .Decision((ct, str, md) => str == "AA", positive, null);

            positive
                .Process((ct, str, md) => str + "B")
                .Action((ct, str, md) => result = str);

            var cts = new CancellationTokenSource();
            head.Push(cts.Token, "A", new PipelineMetadata());
            result.Should().Be("AAB");
        }
        
        [Fact]
        public void Decision_pipeline_has_only_negative_branch()
        {
            var result = string.Empty;

            var head = new HeadBlock<string>();
            var negative = new HeadBlock<string>();
            
            head.Process((ct, str, md) => str + "A")
                .Decision((ct, str, md) => str == "A-", null, negative);

            negative
                .Process((ct, str, md) => str + "B")
                .Action((ct, str, md) => result = str);

            var cts = new CancellationTokenSource();
            head.Push(cts.Token, "A", new PipelineMetadata());
            result.Should().Be("AAB");
        }
    }
}