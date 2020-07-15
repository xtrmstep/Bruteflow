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
            var head = new ProcessBlock<string, string>((str, md) => str + "A");
            head.Process((str, md) => str + "B")
                .Process((str, md) => str + "C")
                .Action((str, md) => result = str);

            head.Process(string.Empty, new PipelineMetadata());
            result.Should().Be("ABC");
        }

        [Fact]
        public void Transformation_block_should_broadcast()
        {
            var result1 = string.Empty;
            var result2 = string.Empty;
            var head = new ProcessBlock<string, string>((str, md) => str + "A",
                new ProcessBlock<string, string>((str, md) => str + "-",
                    new DistributeBlock<string>(
                        new ProcessBlock<string, string>((str, md) => str + "B",
                                new ActionBlock<string>((str, md) => result1 = str)),
                        new ProcessBlock<string, string>((str, md) => str + "C",
                                new ActionBlock<string>((str, md) => result2 = str))
                    )
                )
            );
            /*
             * var head = new ProcessBlock<string, string>(method1)
             *                     .Transform<string, string>(method2)
             *                     .Broadcast<string>(
             *                         new ProcessBlock<string, string>(method3)
             *                             .Sink<string>(method4),
             *                         new ProcessBlock<string, string>(method5)
             *                             .Sink<string>(method6)
             *                      );
             */

            head.Process(string.Empty, new PipelineMetadata());
            result1.Should().Be("A-B");
            result2.Should().Be("A-C");
        }
    }
}