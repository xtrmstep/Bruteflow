using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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

            await head.PushAsync(cts.Token, "C", new PipelineMetadata()).ConfigureAwait(false);
            await head.PushAsync(cts.Token, "C", new PipelineMetadata()).ConfigureAwait(false);
            await head.PushAsync(cts.Token, "C", new PipelineMetadata()).ConfigureAwait(false);
            // this one will be lost because of the batching
            await head.PushAsync(cts.Token, "C", new PipelineMetadata()).ConfigureAwait(false);
            await head.FlushAsync(cts.Token).ConfigureAwait(false);

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
                .Decision((ct, str, md) => 
                    Task.FromResult(str == "AA"), positive, negative);

            positive
                .Process((ct, str, md) => Task.FromResult(str + "B"))
                .Action((ct, str, md) => Task.FromResult(result = str));

            negative
                .Process((ct, str, md) => Task.FromResult(str + "C"))
                .Action((ct, str, md) => Task.FromResult(result = str));

            var cts = new CancellationTokenSource();
            await head.PushAsync(cts.Token, "A", new PipelineMetadata()).ConfigureAwait(false);
            result.Should().Be("AAB");

            await head.PushAsync(cts.Token, "B", new PipelineMetadata()).ConfigureAwait(false);
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
            await head.PushAsync(cts.Token, string.Empty, new PipelineMetadata()).ConfigureAwait(false);

            result1.Should().Be("A-B");
            result2.Should().Be("A-C");
        }

        [Fact]
        public async Task Head_with_only_pipeline_should_execute_all_blocks()
        {
            var result = string.Empty;
            var head = new HeadBlock<string>();
            head.Process((ct, str, md) => Task.FromResult(str + "A"))
                .Process((ct, str, md) => Task.FromResult(str + "B"))
                .Process((ct, str, md) => Task.FromResult(str + "C"))
                .Action((ct, str, md) => Task.FromResult(result = str));

            var cts = new CancellationTokenSource().Token;
            await head.PushAsync(cts, string.Empty, new PipelineMetadata()).ConfigureAwait(false);

            result.Should().Be("ABC");
        }
        
        [Fact]
        public async Task Head_without_pipeline_should_await_start()
        {
            var result = 0;
            
            var head = new HeadBlock<int>(async (token, func) =>
            {
                for (var i = 0; i < 4; i++)
                {
                    await func(token, i, new PipelineMetadata());
                    result += i;
                }
            });

            var cts = new CancellationTokenSource().Token;
            await head.Start(cts).ConfigureAwait(false);

            result.Should().Be(6);
        }
        
        [Fact]
        public async Task Head_with_generator_should_await_blocks_when_have_one()
        {
            var result = 0;
            
            var head = new HeadBlock<int>(async (token, func) =>
            {
                for (var i = 0; i < 4; i++)
                {
                    await func(token, i, new PipelineMetadata());
                }
            });
            head.Action((ct, i, md) => Task.FromResult(result += i));

            var cts = new CancellationTokenSource().Token;
            await head.Start(cts);

            result.Should().Be(6);
        }
        
        [Fact]
        public async Task Head_with_generator_should_await_blocks_when_have_two()
        {
            var result = 0;
            
            var head = new HeadBlock<int>(async (token, func) =>
            {
                for (var i = 0; i < 4; i++)
                {
                    await func(token, i, new PipelineMetadata());
                }
            });
            head.Process((ct, i, md) => Task.FromResult(i + 1))
                .Action((ct, i, md) => Task.FromResult(result += i));

            var cts = new CancellationTokenSource().Token;
            await head.Start(cts);

            result.Should().Be(10);
        }
        
        [Fact]
        public async Task Head_with_generator_should_await_blocks_when_have_many()
        {
            var result = 0;
            
            var head = new HeadBlock<int>(async (token, func) =>
            {
                for (var i = 0; i < 4; i++)
                {
                    await func(token, i, new PipelineMetadata());
                }
            });
            head.Process((ct, i, md) => Task.FromResult(i + 1))
                .Process((ct, i, md) => Task.FromResult(i + 1))
                .Process((ct, i, md) => Task.FromResult(i + 1))
                .Action((ct, i, md) => Task.FromResult(result += i));

            var cts = new CancellationTokenSource().Token;
            await head.Start(cts);

            result.Should().Be(18);
        }
    }
}