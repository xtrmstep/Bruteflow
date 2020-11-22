using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Blocks;
using FluentAssertions;
using Xunit;

namespace Bruteflow.Tests
{
    public class BlockTests
    {
        [Fact]
        public async Task Batch_pipeline_one_input_accumulate_to_one_output()
        {
            var result = new List<string>();

            var head = new HeadBlock<string>();
            head.Process((ct, str, md) => Task.FromResult(str + "A"))
                .Batch(3)
                .Process((ct, str, md) => Task.FromResult(string.Join(",", str)))
                .Action((ct, str, md) =>
                {
                    result.Add(str);
                    return Task.CompletedTask;
                });
            
            var cts = new CancellationTokenSource();

            await Task.WhenAll(new[]
            {
                head.Push(cts.Token, "C", new PipelineMetadata()),
                head.Push(cts.Token, "C", new PipelineMetadata()),
                head.Push(cts.Token, "C", new PipelineMetadata()),
                // this one will be lost because of the batching
                head.Push(cts.Token, "C", new PipelineMetadata()),
                head.Flush(cts.Token)
            });

            result.Count.Should().Be(2);
            result[0].Should().Be("CA,CA,CA");
            result[1].Should().Be("CA");
        }

        [Fact]
        public async Task Decision_pipeline_one_input_different_outputs()
        {
            var result = string.Empty;

            var head = new HeadBlock<string>();
            var positive = new HeadBlock<string>();
            var negative = new HeadBlock<string>();
            head.Process((ct, str, md) => Task.FromResult(str + "A"))
                .Decision((ct, str, md) => Task.FromResult(str == "AA"), positive, negative);

            positive
                .Process((ct, str, md) => Task.FromResult(str + "B"))
                .Action((ct, str, md) => Task.FromResult(result = str));

            negative
                .Process((ct, str, md) => Task.FromResult(str + "C"))
                .Action((ct, str, md) => Task.FromResult(result = str));

            var cts = new CancellationTokenSource();
            await head.Push(cts.Token, "A", new PipelineMetadata());
            result.Should().Be("AAB");

            await head.Push(cts.Token, "B", new PipelineMetadata());
            result.Should().Be("BAC");
        }

        [Fact]
        public async Task Distribute_pipeline_one_input_two_outputs()
        {
            var result1 = string.Empty;
            var result2 = string.Empty;

            var head = new HeadBlock<string>();
            var branch1 = new HeadBlock<string>();
            var branch2 = new HeadBlock<string>();
            head.Process((ct, str, md) => Task.FromResult(str + "A"))
                .Process((ct, str, md) => Task.FromResult(str + "-"))
                .Distribute(branch1, branch2);

            branch1
                .Process((ct, str, md) => Task.FromResult(str + "B"))
                .Action((ct, str, md) => Task.FromResult(result1 = str));

            branch2
                .Process((ct, str, md) => Task.FromResult(str + "C"))
                .Action((ct, str, md) => Task.FromResult(result2 = str));
            
            var cts = new CancellationTokenSource();
            await head.Push(cts.Token, string.Empty, new PipelineMetadata());

            result1.Should().Be("A-B");
            result2.Should().Be("A-C");
        }

        [Fact]
        public async Task Process_pipeline_one_input_one_output()
        {
            var result = string.Empty;
            var head = new HeadBlock<string>();
            head.Process((ct, str, md) => Task.FromResult(str + "A"))
                .Process((ct, str, md) => Task.FromResult(str + "B"))
                .Process((ct, str, md) => Task.FromResult(str + "C"))
                .Action((ct, str, md) => Task.FromResult(result = str));

            var cts = new CancellationTokenSource();
            await head.Push(cts.Token, string.Empty, new PipelineMetadata());

            result.Should().Be("ABC");
        }
    }
}