using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Blocks;
using FluentAssertions;
using Xunit;

namespace Bruteflow.Tests
{
    public class CreationOfBlocksTests
    {        
        [Fact]
        public async Task Decision_pipeline_has_only_positive_branch()
        {
            var result = string.Empty;

            var head = new HeadBlock<string>();
            var positive = new HeadBlock<string>();
            
            head.Process((ct, str, md) => Task.FromResult(str + "A"))
                .Decision((ct, str, md) => Task.FromResult(str == "AA"), positive, null);

            positive
                .Process((ct, str, md) => Task.FromResult(str + "B"))
                .Action((ct, str, md) => Task.FromResult(result = str));

            var cts = new CancellationTokenSource();
            await head.PushAsync(cts.Token, "A", new PipelineMetadata());
            result.Should().Be("AAB");
        }
        
        [Fact]
        public async Task Decision_pipeline_has_only_negative_branch()
        {
            var result = string.Empty;

            var head = new HeadBlock<string>();
            var negative = new HeadBlock<string>();
            
            head.Process((ct, str, md) => Task.FromResult(str + "A"))
                .Decision((ct, str, md) => Task.FromResult(str == "A-"), null, negative);

            negative
                .Process((ct, str, md) => Task.FromResult(str + "B"))
                .Action((ct, str, md) => Task.FromResult(result = str));

            var cts = new CancellationTokenSource();
            await head.PushAsync(cts.Token, "A", new PipelineMetadata());
            result.Should().Be("AAB");
        }
    }
}