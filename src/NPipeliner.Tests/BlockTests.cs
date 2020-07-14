using System;
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
                        new SinkBlock<string>((str, md) => result = str))));
            
            head.Process(string.Empty, new PipelineMetadata());
            result.Should().Be("ABC");
        }
    }
}