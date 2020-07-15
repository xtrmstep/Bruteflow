using Flowcharter.Blocks;
using FluentAssertions;
using Xunit;

namespace Flowcharter.Tests
{
    public class BlockTests
    {
        [Fact]
        public void Transformation_block_should_be_pipelined()
        {
            var result = string.Empty;
            var head = new ProcessBlock<string, string>();
            head.Process((str, md) => str + "A")
                .Next((str, md) => str + "B")
                .Next((str, md) => str + "C")
                .Action((str, md) => result = str);

            head.Post(string.Empty, new PipelineMetadata());
            result.Should().Be("ABC");
        }

        [Fact]
        public void Transformation_block_should_broadcast()
        {
            var result1 = string.Empty;
            var result2 = string.Empty;

            var branch1 = new ProcessBlock<string, string>();
            var branch2 = new ProcessBlock<string, string>();
            var head = new ProcessBlock<string, string>();
            head.Process((str, md) => str + "A")
                .Next((str, md) => str + "-")
                .Distribute(branch1, branch2);
            
            branch1
                .Process((str, md) => str + "B")
                .Action((str, md) => result1 = str);
            
            branch2
                .Process((str, md) => str + "C")
                .Action((str, md) => result2 = str);

            head.Post(string.Empty, new PipelineMetadata());
            result1.Should().Be("A-B");
            result2.Should().Be("A-C");
        }
    }
}