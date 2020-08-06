using System.Collections.Generic;
using System.Threading;
using Bruteflow.Blocks;
using FluentAssertions;
using Xunit;

namespace Bruteflow.Tests
{
    public class BlockTests
    {
        [Fact]
        public void Batch_pipeline_one_input_accumulate_to_one_output()
        {
            var result = new List<string>();

            var head = new HeadBlock<string>();
            head.Process((ct, str, md) => str + "A")
                .Batch(3)
                .Process((ct, str, md) => string.Join(',', str))
                .Action((ct, str, md) => result.Add(str));
            
            var cts = new CancellationTokenSource();

            head.Push(cts.Token, "C", new PipelineMetadata());
            head.Push(cts.Token, "C", new PipelineMetadata());
            head.Push(cts.Token, "C", new PipelineMetadata());
            // this one will be lost because of the batching
            head.Push(cts.Token, "C", new PipelineMetadata());
            head.Flush(cts.Token);

            result.Count.Should().Be(2);
            result[0].Should().Be("CA,CA,CA");
            result[1].Should().Be("CA");
        }

        [Fact]
        public void Decision_pipeline_one_input_different_outputs()
        {
            var result = string.Empty;

            var head = new HeadBlock<string>();
            var positive = new HeadBlock<string>();
            var negative = new HeadBlock<string>();
            head.Process((ct, str, md) => str + "A")
                .Decision((ct, str, md) => str == "AA", positive, negative);

            positive
                .Process((ct, str, md) => str + "B")
                .Action((ct, str, md) => result = str);

            negative
                .Process((ct, str, md) => str + "C")
                .Action((ct, str, md) => result = str);

            var cts = new CancellationTokenSource();
            head.Push(cts.Token, "A", new PipelineMetadata());
            result.Should().Be("AAB");

            head.Push(cts.Token, "B", new PipelineMetadata());
            result.Should().Be("BAC");
        }

        [Fact]
        public void Distribute_pipeline_one_input_two_outputs()
        {
            var result1 = string.Empty;
            var result2 = string.Empty;

            var head = new HeadBlock<string>();
            var branch1 = new HeadBlock<string>();
            var branch2 = new HeadBlock<string>();
            head.Process((ct, str, md) => str + "A")
                .Process((ct, str, md) => str + "-")
                .Distribute(branch1, branch2);

            branch1
                .Process((ct, str, md) => str + "B")
                .Action((ct, str, md) => result1 = str);

            branch2
                .Process((ct, str, md) => str + "C")
                .Action((ct, str, md) => result2 = str);
            
            var cts = new CancellationTokenSource();
            head.Push(cts.Token, string.Empty, new PipelineMetadata());

            result1.Should().Be("A-B");
            result2.Should().Be("A-C");
        }

        [Fact]
        public void Process_pipeline_one_input_one_output()
        {
            var result = string.Empty;
            var head = new HeadBlock<string>();
            head.Process((ct, str, md) => str + "A")
                .Process((ct, str, md) => str + "B")
                .Process((ct, str, md) => str + "C")
                .Action((ct, str, md) => result = str);

            var cts = new CancellationTokenSource();
            head.Push(cts.Token, string.Empty, new PipelineMetadata());

            result.Should().Be("ABC");
        }
    }
}