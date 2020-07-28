# Bruteflow

Simple synchronous pipeline builder which supports well-known flowchart blocks (start, process, decision) and enhance them with additional blocks: distribute and batch.

## Blocks

Any Bruteflow pipeline starts with HeadBlock block and ends with ActionBlock. Bruteflow with its extension methods help to build DAG pipeline.

The execution of the pipeline is synchroneous, but code inside blocks may be asynchroneous .

### HeadBlock

Starting block of a pipeline. You can push data or specify a method which will produce series of data.

You need to specify a type which will be produced by the block.

### ProcessBlock

The block receives and produces data. Type of incoming and outcoming data may be different.

### DecisionBlock

The block which uses condition to route data to different branches. You need to specify the condition and use head or processing blocks to propogate data in the pipeline.

### DistributeBlock

The block which broadcast incoming data to all following blocks which has been linked. Every following block will recevie same data.

### BatchBlock

The block has internal state and stores data in bunches. In order to propogate data when batch is incomplete, you ned to flush the pipeline. Flushing will prpogate internal data further in the pipeline and clear internal states.

### ActionBlock

The final block of the pipeline. It doesn't propogate and should be used as an ending block of the pipeline.

