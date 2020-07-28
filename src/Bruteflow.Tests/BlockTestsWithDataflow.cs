using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Xunit;

namespace Bruteflow.Tests
{
    public class BlockTestsWithDataflow
    {
        [Fact]
        public void Batch_pipeline_one_input_accumulate_to_one_output()
        {
            var result = new List<string>();

            var head = new TransformBlock<string, string>(str => str + "A");
            var block2 = new BatchBlock<string>(3);
            var block3 = new TransformBlock<string[], string>(str => string.Join(',', str));
            var block4 = new ActionBlock<string>(str => result.Add(str));

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            head.LinkTo(block2, linkOptions);
            block2.LinkTo(block3, linkOptions);
            block3.LinkTo(block4, linkOptions);

            head.Post("C");
            head.Post("C");
            head.Post("C");
            // this one will be lost because of the batching
            head.Post("C");
            head.Complete();
            block4.Completion.Wait();

            result.Count.Should().Be(2);
            result[0].Should().Be("CA,CA,CA");
            result[1].Should().Be("CA");
        }

        [Fact]
        public void Distribute_pipeline_one_input_two_outputs()
        {
            var result1 = string.Empty;
            var result2 = string.Empty;

            var head = new TransformBlock<string, string>(str => str + "A");
            var block2 = new TransformBlock<string, string>(str => str + "-");
            var block3 = new BroadcastBlock<string>(str => str);
            var block4 = new TransformBlock<string, string>(str => str + "B");
            var block5 = new TransformBlock<string, string>(str => str + "C");
            var block6 = new ActionBlock<string>(str => result1 = str);
            var block7 = new ActionBlock<string>(str => result2 = str);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            head.LinkTo(block2, linkOptions);
            block2.LinkTo(block3, linkOptions);
            block3.LinkTo(block4, linkOptions);
            block4.LinkTo(block6, linkOptions);
            block3.LinkTo(block5, linkOptions);
            block5.LinkTo(block7, linkOptions);

            head.Post(string.Empty);
            head.Complete();
            block6.Completion.Wait();
            block7.Completion.Wait();

            result1.Should().Be("A-B");
            result2.Should().Be("A-C");
        }

        [Fact]
        public void Process_pipeline_one_input_one_output()
        {
            var result = string.Empty;
            var head = new TransformBlock<string, string>(str => str + "A");
            var block2 = new TransformBlock<string, string>(str => str + "B");
            var block3 = new TransformBlock<string, string>(str => str + "C");
            var block4 = new ActionBlock<string>(str => result = str);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            head.LinkTo(block2, linkOptions);
            block2.LinkTo(block3, linkOptions);
            block3.LinkTo(block4, linkOptions);

            head.Post(string.Empty);
            head.Complete();
            block4.Completion.Wait();

            result.Should().Be("ABC");
        }
    }
}