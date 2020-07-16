using System.Threading.Tasks.Dataflow;
using Flowcharter.Blocks;
using FluentAssertions;
using Xunit;

namespace Flowcharter.Tests
{
    public class BlockTestsWithDataflow
    {
        [Fact]
        public void Strait_pipeline_one_input_one_output()
        {
            var result = string.Empty;
            var head = new TransformBlock<string, string>(str => str + "A");
            var block2 = new TransformBlock<string, string>(str => str + "B");
            var block3 = new TransformBlock<string, string>(str => str + "C");
            var block4 = new System.Threading.Tasks.Dataflow.ActionBlock<string>(str => result = str);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            head.LinkTo(block2, linkOptions);
            block2.LinkTo(block3, linkOptions);
            block3.LinkTo(block4, linkOptions);

            head.Post(string.Empty);
            head.Complete();
            block4.Completion.Wait();
            
            result.Should().Be("ABC");
        }

        // [Fact]
        // public void Fork_pipeline_one_input_two_outputs()
        // {
        //     var result1 = string.Empty;
        //     var result2 = string.Empty;
        //
        //     var branch1 = new ProcessBlock<string, string>();
        //     var branch2 = new ProcessBlock<string, string>();
        //     var head = new ProcessBlock<string, string>();
        //     head.Process((str, md) => str + "A")
        //         .Next((str, md) => str + "-")
        //         .Distribute(branch1, branch2);
        //     
        //     branch1
        //         .Process((str, md) => str + "B")
        //         .Action((str, md) => result1 = str);
        //     
        //     branch2
        //         .Process((str, md) => str + "C")
        //         .Action((str, md) => result2 = str);
        //
        //     head.Push(string.Empty, new PipelineMetadata());
        //     result1.Should().Be("A-B");
        //     result2.Should().Be("A-C");
        // }
        //
        // [Fact]
        // public void Decision_pipeline_one_input_different_outputs()
        // {
        //     var result = string.Empty;
        //
        //     var positive = new ProcessBlock<string, string>();
        //     var negative = new ProcessBlock<string, string>();
        //     var head = new ProcessBlock<string, string>();
        //     head.Process((str, md) => str + "A")
        //         .Decision((str, md) => str == "AA", positive, negative);
        //     
        //     positive
        //         .Process((str, md) => str + "B")
        //         .Action((str, md) => result = str);
        //     
        //     negative
        //         .Process((str, md) => str + "C")
        //         .Action((str, md) => result = str);
        //
        //     head.Push("A", new PipelineMetadata());
        //     result.Should().Be("AAB");
        //     
        //     head.Push("B", new PipelineMetadata());
        //     result.Should().Be("BAC");
        //
        // }
        //
        // [Fact]
        // public void Batch_pipeline_one_input_accumulate_to_one_output()
        // {
        //     var result = string.Empty;
        //
        //     var head = new ProcessBlock<string, string>();
        //     head.Process((str, md) => str + "A")
        //         .Batch(3)
        //         .Next((str, md) => string.Join(',', str))
        //         .Action((str, md) => result = str);
        //
        //     head.Push("C", new PipelineMetadata());
        //     head.Push("C", new PipelineMetadata());
        //     head.Push("C", new PipelineMetadata());
        //     // this one will be lost because of the batching
        //     head.Push("C", new PipelineMetadata());
        //     
        //     result.Should().Be("CA,CA,CA");
        // }
    }
}