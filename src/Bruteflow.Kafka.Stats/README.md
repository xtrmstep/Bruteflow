# Bruteflow

Simple synchronous pipeline builder which supports well-known flowchart blocks (start, process, decision) and enhance them with additional blocks: distribute and batch. This is no way a replacement for Dataflow (Task Parallel Library). It uses another approach for building a pipeline.

## Blocks

Any Bruteflow pipeline starts with HeadBlock block and ends with ActionBlock. Bruteflow with its extension methods help to build DAG pipeline. The execution of the pipeline is synchronous, but code inside blocks may be asynchronous .

| Block | Description |
| ------ | ------ |
| HeadBlock | Starting block of a pipeline. You can push data or specify a method which will produce series of data. You need to specify a type which will be produced by the block. |
| ProcessBlock | The block receives and produces data. Type of incoming and outgoing data may be different. |
| DecisionBlock | The block which uses condition to route data to different branches. You need to specify the condition and use head or processing blocks to propagate data in the pipeline. |
| DistributeBlock | The block which broadcast incoming data to all following blocks which has been linked. Every following block will receive same data. |
| BatchBlock | The block has internal state and stores data in bunches. In order to propagate data when batch is incomplete, you ned to flush the pipeline. Flushing will propagate internal data further in the pipeline and clear internal states. |
| ActionBlock | The final block of the pipeline. It doesn't propagate and should be used as an ending block of the pipeline. |

# Examples

The construction of a pipeline with batch block, pushing values into the pipeline and reading results.

```c#
var result = new List<string>();

var head = new HeadBlock<string>();
head.Process((str, md) => str + "A")
    .Batch(3)
    .Process((str, md) => string.Join(',', str))
    .Action((str, md) => result.Add(str));

head.Push("C", new PipelineMetadata());
head.Push("C", new PipelineMetadata());
head.Push("C", new PipelineMetadata());
// this one will be lost because of the batching
head.Push("C", new PipelineMetadata());
head.Flush();

result.Count.Should().Be(2);
result[0].Should().Be("CA,CA,CA");
result[1].Should().Be("CA");
```

More examples you can find in tests. Also you will find there similar tests written with [Dataflow (Task Parallel Library)](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library)
