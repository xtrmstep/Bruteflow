using FluentAssertions;
using NPipeliner.Blocks;
using Xunit;

namespace NPipeliner.Tests
{
    public class BlockTests
    {
        [Fact]
        public void Transformation_block_should_be_pipelined()
        {
            var result = string.Empty;
            var head = new TransformationBlock<string, string>((str, md) => str + "A",
                new TransformationBlock<string, string>((str, md) => str + "B",
                    new TransformationBlock<string, string>((str, md) => str + "C",
                        new SinkBlock<string>((str, md) => result = str)
                    )
                )
            );

            head.Process(string.Empty, new PipelineMetadata());
            result.Should().Be("ABC");
        }

        [Fact]
        public void Transformation_block_should_broadcast()
        {
            var result1 = string.Empty;
            var result2 = string.Empty;
            var head = new TransformationBlock<string, string>((str, md) => str + "A",
                new TransformationBlock<string, string>((str, md) => str + "-",
                    new TransformationBlock<string, string>((str, md) => str + "B", new SinkBlock<string>((str, md) => result1 = str)),
                    new TransformationBlock<string, string>((str, md) => str + "C", new SinkBlock<string>((str, md) => result2 = str))
                )
            );

            head.Process(string.Empty, new PipelineMetadata());
            result1.Should().Be("A-B");
            result2.Should().Be("A-C");
        }
    }
}