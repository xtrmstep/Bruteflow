using Flowcharter.Blocks;
using FluentAssertions;
using Xunit;

namespace Flowcharter.Tests
{
    public class BlockTests
    {
        [Fact]
        public void Strait_pipeline_one_input_one_output()
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
        public void Fork_pipeline_one_input_two_outputs()
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

        [Fact]
        public void Decision_pipeline_one_input_different_outputs()
        {
            var result = string.Empty;

            var positive = new ProcessBlock<string, string>();
            var negative = new ProcessBlock<string, string>();
            var head = new ProcessBlock<string, string>();
            head.Process((str, md) => str + "A")
                .Decision((str, md) => str == "AA", positive, negative);
            
            positive
                .Process((str, md) => str + "B")
                .Action((str, md) => result = str);
            
            negative
                .Process((str, md) => str + "C")
                .Action((str, md) => result = str);

            head.Post("A", new PipelineMetadata());
            result.Should().Be("AAB");
            
            head.Post("B", new PipelineMetadata());
            result.Should().Be("BAC");

        }
    }
}