![Publish Bruteflow NuGet Package](https://github.com/xtrmstep/Bruteflow/workflows/Publish%20Bruteflow%20NuGet%20Package/badge.svg)

![Publish Bruteflow.Kafka NuGet Package](https://github.com/xtrmstep/Bruteflow/workflows/Publish%20Bruteflow.Kafka%20NuGet%20Package/badge.svg)

![Publish Bruteflow.Kafka.Stats NuGet Package](https://github.com/xtrmstep/Bruteflow/workflows/Publish%20Bruteflow.Kafka.Stats%20NuGet%20Package/badge.svg)

# Bruteflow libraries

Simple synchronous pipeline builder which supports well-known flowchart blocks and offers basic bulding blocks to construct Kafka-based DAG pipeline.

This is no way a replcement for Dataflow (Task Parallel Library). It uses another approach for building a pipeline.

## Libraries

| Library | Description |
| ------ | ------ |
| [Bruteflow](src/Bruteflow/README.md) | Basic building blocks to contruct a dataflow pipeline. |
| [Bruteflow.Kafka](src/Bruteflow.Kafka/README.md) | The block receives and produces data. Type of incoming and outcoming data may be different. |
